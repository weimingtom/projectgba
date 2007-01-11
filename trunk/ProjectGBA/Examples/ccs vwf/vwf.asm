;============================
;   Card Captor Sakura VWF
;      v1.0 by Spikeman
;============================

@thumb
@textarea 0x08FCEF10

vwf
	cmp r3,#0x0
	bgt notfirst
	strb r3,[r7, #0xB]	;r3 is already 0, use it to zero width
notfirst
	push {r1,r3}
	ldrb r1,[r7, #0xB]
	mov r0,r1
	ldr r2,[r7, #0x50]	;address pointing to next char (4c)
	sub r2, #0x2		;current char address
	ldrb r2,[r2]		;load char
	ldr r3,[widthtbla+2]	;get widthtable
	add r3,r2,r3		;get address to char width
	ldrb r3,[r3]		;load char width
	add r1,r1,r3		;add to current width
	strb r1,[r7, #0xB]	;store next width
				;r0 is current width still
	pop {r1,r3}

originalcode
	lsl r0,r4
	add r5,r5,r0
	lsr r1,r1,#0x10
	ldrh r2,[r7,#0x18]
	add r1,r1,r2
	ldrb r0,[r7,#0x0C]
	mul r0,r6
	lsl r0,r4

	mov pc,lr		;return

widthtbla
@dcd widthtable
widthtable

@endarea