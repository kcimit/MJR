# MJR

Utility to extract JPEG files from MotionJPEG (MJPEG) containers normally found in Canon video files (.MOV) uncluding broken ones (.DAT).

Typical use scenario is when camera unexpectedly switched off while filiming (battery off, camera malfunction etc.) or simpy no more memory to write .MOV sequence - which results in .DAT file generated on memory card.

Ectracted files could be merged into final sequence using ffmpeg.
Procedure as follows:
1. Extract files using MJR
2. Remove invalid files (normally identified by their small size)
3. Rename final sequence using sequential numbering 0000.jpeg to XXXX.jpeg (there are many tools available to achieve this)
4. Merge using ffmpeg, for example for 4K Canon 5D Mark IV cmd line would look like:
    ffmpeg -r 25 -f image2 -s 4096x2160 -i %04d.jpeg -vcodec libx264 -crf 25 test.mp4
   
