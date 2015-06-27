REM This will clean up the built up files in order to make
REM it easy to not to pollute the source code control system
::

del PDFWriter.suo  /A:H /F /Q

del .\zOutputFiles\pdf-sample.pdf /F /Q
rd zOutputFiles

del .\PDFWriter\bin\Debug\*.* /F /Q
rd  .\PDFWriter\bin\Debug

del .\PDFWriter\bin\*.* /F /Q
rd  .\PDFWriter\bin

del .\PDFWriter\obj\x86\Debug\TempPE\*.* /F /Q
rd  .\PDFWriter\obj\x86\Debug\TempPE

del .\PDFWriter\obj\x86\Debug\*.* /F /Q
rd  .\PDFWriter\obj\x86\Debug

del .\PDFWriter\obj\x86\*.* /F /Q
rd  .\PDFWriter\obj\x86

del .\PDFWriter\obj\*.* /F /Q
rd  .\PDFWriter\obj

pause