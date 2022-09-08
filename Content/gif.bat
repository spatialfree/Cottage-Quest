ffmpeg -f image2 -framerate 15 -i %%04d.png output.gif
:PROMPT
@echo  off
SET /P OPENFILE=open file (y/N)?
IF /I "%OPENFILE%" NEQ "Y" GOTO END
output.gif
:END
