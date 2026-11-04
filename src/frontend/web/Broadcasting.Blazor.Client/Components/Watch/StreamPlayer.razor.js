let hlsInstance = null;
let uiHideTimer = null;
let retryTimer = null;
let abortController = null;

export function init()
{
    dispose();

    abortController = new AbortController();
    const { signal } = abortController;

    const playerEl = document.getElementById('main-stream-player');
    const video = document.getElementById('videoPlayer');
    const statusOverlay = document.getElementById('statusOverlay');
    const centerPlayAction = document.getElementById('player-center-play');
    const centerLabel = document.getElementById('player-center-label');
    const bottomPlayBtn = document.getElementById('player-bottom-play');
    const playLabel = document.getElementById('player-play-label');
    const liveSyncChip = document.getElementById('player-live-sync');
    const liveChipText = document.getElementById('player-live-chip-text');
    const syncReadout = document.getElementById('player-sync-readout');
    const muteBtn = document.getElementById('player-mute-btn');
    const volumeSlider = document.getElementById('player-volume-slider');
    const volumeValue = document.getElementById('player-volume-value');
    const fullscreenBtn = document.getElementById('player-fullscreen-btn');

    let isPlaying = false;
    let pendingLiveEdgeSync = false;
    let lastVolume = 50;

    const rawStreamUrl = video.dataset.streamSrc;
    const streamUrl = rawStreamUrl.endsWith('.m3u8')
        ? rawStreamUrl
        : `${rawStreamUrl.replace(/\/+$/, '')}/index.m3u8`;

    // Calculate and jump directly to the live stream edge (accounting for buffer & latency)
    const syncToLiveEdge = () => {
        try {
            if (hlsInstance && Number.isFinite(hlsInstance.liveSyncPosition) && hlsInstance.liveSyncPosition > 0) {
                video.currentTime = hlsInstance.liveSyncPosition;
                return;
            }

            if (video.buffered && video.buffered.length > 0) {
                const lastIdx = video.buffered.length - 1;
                const bufferedStart = video.buffered.start(lastIdx);
                const bufferedEnd = video.buffered.end(lastIdx);
                if (Number.isFinite(bufferedEnd) && bufferedEnd > 0) {
                    video.currentTime = Math.max(bufferedStart, bufferedEnd - 2.0);
                }
            }

        } catch (_) {
            // Ignore seek errors before media metadata is ready
        }
    };

    const updatePlayerUI = (playing) => {
        isPlaying = playing;
        playerEl.classList.toggle('is-playing', isPlaying);

        if (playLabel) {
            playLabel.textContent = isPlaying ? 'PAUSE' : 'PLAY';
        }

        if (liveSyncChip && liveChipText) {
            if (isPlaying) {
                liveSyncChip.classList.remove('live-status-chip--behind');
                liveChipText.textContent = 'LIVE';
            } else {
                liveSyncChip.classList.add('live-status-chip--behind');
                liveChipText.textContent = 'PAUSED';
            }
        }

        if (syncReadout) {
            syncReadout.textContent = isPlaying ? 'SYNC // LIVE EDGE' : 'BUFFER // PAUSED';
        }

        if (!isPlaying && centerLabel) {
            centerLabel.textContent = 'RESUME STREAM [LIVE EDGE]';
        }
    };

    const playFromLiveEdge = _ => {
        pendingLiveEdgeSync = true;

        if (hlsInstance) {
            // Restart segment loading at the live edge (-1) so stale buffer is skipped
            hlsInstance.startLoad(-1);
        }

        syncToLiveEdge();
        updatePlayerUI(true);

        const playPromise = video.play();
        if (playPromise && typeof playPromise.catch === 'function') {
            playPromise.catch(() => {
                // Keep UI interactive even if local ingest server on :8888 is not active
            });
        }
    };

    const pauseStream = _ => {
        pendingLiveEdgeSync = false;
        video.pause();

        if (hlsInstance) {
            // Stop buffering old segments while paused so next Play jumps straight to live edge
            hlsInstance.stopLoad();
        }

        updatePlayerUI(false);
    };

    const togglePlayback = () => {
        if (isPlaying) {
            pauseStream(true);
        } else {
            playFromLiveEdge(true);
        }
    };

    // Initialize HLS.js or native HLS pipeline
    if (typeof window.Hls !== 'undefined' && window.Hls.isSupported()) {
        hlsInstance = new window.Hls({
            manifestLoadingMaxRetry: Infinity,
            manifestLoadingRetryDelay: 1000,
            liveSyncDuration: 2,
            maxLiveSyncPlaybackRate: 1.15,
            backBufferLength: 0,
            maxBufferLength: 30,
            maxMaxBufferLength: 60,
            maxBufferSize: 150 * 1024 * 1024,
            enableWorker: true,
            lowLatencyMode: true
        });

        hlsInstance.loadSource(streamUrl);
        hlsInstance.attachMedia(video);

        hlsInstance.on(window.Hls.Events.MANIFEST_PARSED, () => {
            if (statusOverlay) {
                statusOverlay.textContent = 'LIVE BROADCAST';
                statusOverlay.classList.remove('badge-live--waiting');
                statusOverlay.classList.add('badge-live--online');
            }
            playerEl.classList.add('has-video-frames');
            playFromLiveEdge(false);
        });

        hlsInstance.on(window.Hls.Events.FRAG_BUFFERED, () => {
            if (pendingLiveEdgeSync && isPlaying) {
                syncToLiveEdge();
                pendingLiveEdgeSync = false;
            }
        });

        hlsInstance.on(window.Hls.Events.ERROR, (event, data) => {
            if (data && data.fatal) {
                if (statusOverlay) {
                    statusOverlay.textContent = 'WAITING FOR INGEST...';
                    statusOverlay.classList.remove('badge-live--online');
                    statusOverlay.classList.add('badge-live--waiting');
                }
                if (retryTimer) {
                    clearTimeout(retryTimer);
                }
                retryTimer = setTimeout(() => {
                    if (hlsInstance) {
                        hlsInstance.loadSource(streamUrl);
                    }
                }, 2000);
            }
        });
    } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = streamUrl;
        video.addEventListener('loadedmetadata', () => {
            if (statusOverlay) {
                statusOverlay.textContent = 'LIVE BROADCAST';
                statusOverlay.classList.remove('badge-live--waiting');
                statusOverlay.classList.add('badge-live--online');
            }
            playerEl.classList.add('has-video-frames');
            playFromLiveEdge(false);
        }, { signal });
    }

    video.addEventListener('loadeddata', () => {
        playerEl.classList.add('has-video-frames');
    }, { signal });

    centerPlayAction.addEventListener('click', togglePlayback, { signal });
    bottomPlayBtn.addEventListener('click', togglePlayback, { signal });
    video.addEventListener('click', togglePlayback, { signal });

    // Volume Slider & Mute / Unmute Audio Controller
    const volOnIcon = muteBtn ? muteBtn.querySelector('.icon-volume-on') : null;
    const volMutedIcon = muteBtn ? muteBtn.querySelector('.icon-volume-muted') : null;

    const syncVolumeUI = () => {
        const effectiveVol = video.muted ? 0 : Math.round(video.volume * 100);
        const isMuted = video.muted || effectiveVol === 0;

        if (volOnIcon) volOnIcon.style.display = isMuted ? 'none' : 'block';
        if (volMutedIcon) volMutedIcon.style.display = isMuted ? 'block' : 'none';

        if (volumeSlider) {
            volumeSlider.value = String(effectiveVol);
            volumeSlider.style.setProperty('--volume-fill', `${effectiveVol}%`);
        }

        if (volumeValue) {
            volumeValue.textContent = `${effectiveVol}%`;
            volumeValue.style.color = isMuted ? 'var(--text-muted)' : 'var(--accent-acid)';
        }
    };

    const toggleMute = () => {
        if (video.muted || video.volume === 0) {
            const restored = lastVolume > 0 ? lastVolume : 80;
            video.volume = restored / 100;
            video.muted = false;
        } else {
            lastVolume = Math.round(video.volume * 100) || 80;
            video.muted = true;
        }
        syncVolumeUI();
    };

    syncVolumeUI();

    if (muteBtn) {
        muteBtn.addEventListener('click', toggleMute, { signal });
    }

    if (volumeSlider) {
        volumeSlider.addEventListener('input', () => {
            const val = parseInt(volumeSlider.value, 10) || 0;
            video.volume = val / 100;
            if (val === 0) {
                video.muted = true;
            } else {
                video.muted = false;
                lastVolume = val;
            }
            syncVolumeUI();
        }, { signal });
    }

    // Fullscreen Toggle & 3-Second Idle UI Auto-Hide
    const isPlayerFullscreen = () => {
        return Boolean(
            document.fullscreenElement === playerEl ||
            document.webkitFullscreenElement === playerEl
        );
    };

    const toggleFullscreen = () => {
        if (!isPlayerFullscreen()) {
            playerEl.requestFullscreen?.().catch(() => { });
        } else {
            document.exitFullscreen?.().catch(() => { });
        }
    };

    const resetFullscreenIdleTimer = () => {
        playerEl.classList.remove('is-ui-hidden');
        if (uiHideTimer) {
            clearTimeout(uiHideTimer);
            uiHideTimer = null;
        }
        if (isPlayerFullscreen()) {
            uiHideTimer = setTimeout(() => {
                if (isPlayerFullscreen()) {
                    playerEl.classList.add('is-ui-hidden');
                }
            }, 3000);
        }
    };

    document.addEventListener('fullscreenchange', () => {
        if (isPlayerFullscreen()) {
            resetFullscreenIdleTimer();
        } else {
            if (uiHideTimer) {
                clearTimeout(uiHideTimer);
                uiHideTimer = null;
            }
            playerEl.classList.remove('is-ui-hidden');
        }
    }, { signal });

    ['mousemove', 'pointermove', 'mousedown', 'touchstart', 'keydown', 'click'].forEach(evtName => {
        playerEl.addEventListener(evtName, resetFullscreenIdleTimer, { passive: true, signal });
    });

    if (fullscreenBtn) {
        fullscreenBtn.addEventListener('click', toggleFullscreen, { signal });
    }

    // Keyboard Hotkeys: Space (Play/Pause), M (Mute/Unmute), F (Fullscreen)
    document.addEventListener('keydown', (e) => {
        if (isPlayerFullscreen()) {
            resetFullscreenIdleTimer();
        }

        if (e.repeat || e.ctrlKey || e.altKey || e.metaKey) return;

        // Ignore hotkeys when typing in text inputs, textareas, selects, or when a modal is open
        const activeEl = document.activeElement;
        if (activeEl) {
            const tag = activeEl.tagName;
            const isTextInput =
                tag === 'TEXTAREA' ||
                tag === 'SELECT' ||
                activeEl.isContentEditable ||
                (tag === 'INPUT' && activeEl.type !== 'range' && activeEl.type !== 'button');
            if (isTextInput) return;
        }

        if (document.querySelector('.cyber-modal-backdrop.is-open')) return;

        if (e.code === 'Space' || e.key === ' ') {
            e.preventDefault();
            togglePlayback();
        } else if (e.code === 'KeyM' || e.key.toLowerCase() === 'm') {
            e.preventDefault();
            toggleMute();
        } else if (e.code === 'KeyF' || e.key.toLowerCase() === 'f') {
            e.preventDefault();
            toggleFullscreen();
        }
    }, { signal });
}

export function dispose() {
    if (abortController) {
        abortController.abort();
        abortController = null;
    }

    if (uiHideTimer) {
        clearTimeout(uiHideTimer);
        uiHideTimer = null;
    }
    if (retryTimer) {
        clearTimeout(retryTimer);
        retryTimer = null;
    }

    if (hlsInstance) {
        hlsInstance.destroy();
        hlsInstance = null;
    }

    const video = document.getElementById('videoPlayer');
    if (video) {
        video.pause();
        video.removeAttribute('src');
        video.load();
    }

    if (document.fullscreenElement) {
        document.exitFullscreen?.().catch(() => { });
    }
}