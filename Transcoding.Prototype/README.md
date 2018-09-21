# Transcoding.Prototype
Transcode Audio and produce the associated wavform for it.

Currently a WIP, nothing to see here.. currently testing the feasability of ffmpeg transcoding with wavform production.

TODO -> ping when ffmpeg is done to manager

Process wrapper code adapted from [MediaToolkit
](https://github.com/AydinAdn/MediaToolkit).





MAC OSX output 
```Guessed Channel Layout for Input Stream #0.0 : stereo
Input #0, wav, from '0.wav':
  Metadata:
    encoded_by      : Pro Tools
    originator_reference: aainJRYuchRk
    date            : 2018-02-15
    creation_time   : 18:39:53
    time_reference  : 2839636800
  Duration: 00:03:49.68, bitrate: 2316 kb/s
    Stream #0:0: Audio: pcm_s24le ([1][0][0][0] / 0x0001), 48000 Hz, stereo, s32 (24 bit), 2304 kb/s
Stream mapping:
  Stream #0:0 -> #0:0 (pcm_s24le (native) -> mp3 (libmp3lame))
Press [q] to stop, [?] for help
Output #0, mp3, to '11.mp3':
  Metadata:
    TENC            : Pro Tools
    originator_reference: aainJRYuchRk
    TDRC            : 2018-02-15
    time_reference  : 2839636800
    TSSE            : Lavf58.12.100
    Stream #0:0: Audio: mp3 (libmp3lame), 48000 Hz, stereo, s32p (24 bit)
    Metadata:
      encoder         : Lavc58.18.100 libmp3lame
[libmp3lame @ 0x7fdb12814200] Trying to remove 1152 samples, but the queue is empty
size=    3590kB time=00:03:49.70 bitrate= 128.0kbits/s speed=47.1x
video:0kB audio:3590kB subtitle:0kB other streams:0kB global headers:0kB muxing overhead: 0.009849%
```

Windows Output
```
TBD
```