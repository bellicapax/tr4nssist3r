<CsoundSynthesizer>
<CsOptions>
-n -d -Q3
</CsOptions>
<CsInstruments>
; Initialize the global variables.
sr = 44100
ksmps = 32
nchnls = 2
0dbfs = 1

; Globals
giNoteOffset = 36
giNumButtons = 64
gSOnColorFormat init "OnColor%d"
gSOffColorFormat init "OffColor%d"
gSStatusFormat init "Status%d"
gSOpCodeChannel init "OpCode"
giOnColors[] init giNumButtons
giOffColors[] init giNumButtons
giStatuses[] init giNumButtons
; []on colors (kdata2)
; []off colors (kdata2)
; []last codes (kstatus)
; start note addend (add this to index to get note [kdata1] to send)
; operation is kInt that tells you to showMap, wipeMap, listenToController, doNothing

chn_k gSOpCodeChannel, 3, 1
indx init 0
declareMapChannels:
    SOnColorChn sprintf gSOnColorFormat, indx
    SOffColorChn sprintf gSOffColorFormat, indx
    SStatusChn sprintf gSStatusFormat, indx
    chn_k SOnColorChn, 3, 1
    chn_k SOffColorChn, 3, 1
    chn_k SStatusChn, 3, 1
    loop_lt indx, 1, giNumButtons, declareMapChannels


instr 1

kk = 0

;iCode init 1
;chnset iCode, gSOpCodeChannel

;kOpCode = 1
kOpCode chnget gSOpCodeChannel
printks "OpCode %d\n", 1, kOpCode

if (kOpCode == 1) then

  chnset 0, gSOpCodeChannel

  printks "UpdateGrid\n", 1
	updateGrid:
		SStatusChn sprintfk gSStatusFormat, kk
    SOffColorChn sprintfk gSOffColorFormat, kk
    kStatus chnget SStatusChn
    kColor chnget SOffColorChn
    midiout kStatus, 1, kk + giNoteOffset, kColor
    loop_lt kk, 1, giNumButtons, updateGrid


elseif (kOpCode == 2) then

			printks "OpCode%d", 1, kOpCode

endif

endin

</CsInstruments>
<CsScore>
i1 0 30
</CsScore>
</CsoundSynthesizer>
<bsbPanel>
 <label>Widgets</label>
 <objectName/>
 <x>100</x>
 <y>100</y>
 <width>320</width>
 <height>240</height>
 <visible>true</visible>
 <uuid/>
 <bgcolor mode="nobackground">
  <r>255</r>
  <g>255</g>
  <b>255</b>
 </bgcolor>
</bsbPanel>
<bsbPresets>
</bsbPresets>
