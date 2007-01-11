; Card Captor Sakura - Jump to VWF
; by Spikeman

@thumb
@textarea 0x0806688C

	push {r2,lr}
	ldr r2, [jumpaddress+2]
	mov lr, pc
	bx r2
	pop {r2}
	mov r14,r2
	pop {r2}
	b after

jumpaddress
@dcd #0x08FCEF10|1

after

@endarea