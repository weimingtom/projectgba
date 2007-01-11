; Card Captor Sakura Fix - OAM-check
; by normmatt

@thumb
@textarea 0x08FCF340
; r1 is where the XOR value 0xD000 is stored
; r2 is where the tilenumber is located
; r12 is the destination address

check					; Check if the tile number is above 0x3FC
	pop {r2}
	and r0,r1			; Copy 0xD000 into r0
	ldr r3, [maxoam] 		; Load 0x3FC into r3
	cmp r2,r3			; Check if r2 = r3
	bhi resetoam			; If r2 is equal or larger jump to resetoam

originalcode	; Execute original code
	orr r2,r0			; XOR r2 and r0 to make a 0xDxxx value
	mov r0,r12			; Copy destination address to r0
	strh r2,[r0]			; Write value in r2 into the addess in r0
	pop {r3,r4}			; Pop registers 3 and 4
	mov r8,r3			; Copy r3 into r8
	mov r9,r4			; Copy r4 into r9
	pop {r4-r7}			; Pop registers 4 through 7
	pop {r0}			; Pop r0
	bx r0				; Jump back to original code

resetoam				; This routine subtract 0x300 from the tile number as 0x3FC is the maximum tilenumber
	sub r2, #0xFF			; Subtract 0xFF from the tile number
	sub r2, #0xFF			; Subtract 0xFF from the tile number
	sub r2, #0xFF			; Subtract 0xFF from the tile number
	sub r2, #0xFF			; Subtract 0xFF from the tile number
	sub r2, #0x4			; Subtract 0x04 from the tile number(should of taken 0x300 bytes off)
	b originalcode			; Jump back to the original code

maxoam
@dcd #0x000003FC			; This is the maximum tile value and the last tile OAM


@endarea