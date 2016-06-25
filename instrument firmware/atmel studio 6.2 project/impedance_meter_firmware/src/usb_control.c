/*
 * usb_control.c
 *
 * Created: 25.6.2016 11:20:05
 *  Author: user
 */

#include <asf.h>
#include "usb_control.h"
#include "ui.h"
#include "clock_divider.h"
#include "AD5933.h"

#define RECEIVE_BUFFER_SIZE 10
#define SEND_BUFFER_SIZE 10

COMPILER_WORD_ALIGNED static uint8_t recv_buffer[RECEIVE_BUFFER_SIZE];
COMPILER_WORD_ALIGNED static uint8_t send_buffer[SEND_BUFFER_SIZE];
static uint8_t bRequest = 0;

bool main_setup_out_received(void)
{
	ui_connection_state(true);
	bRequest = udd_g_ctrlreq.req.bRequest;
	udd_g_ctrlreq.payload = recv_buffer;
	udd_g_ctrlreq.payload_size = min(udd_g_ctrlreq.req.wLength, sizeof(recv_buffer));
	return true;
}

bool main_setup_in_received(void)
{
	if(bRequest != udd_g_ctrlreq.req.bRequest) return false;
	
	switch(bRequest)
	{
		case 0: // Ping
		{
			for(uint8_t i = 0; i < udd_g_ctrlreq.req.wLength; i++)
			{
				send_buffer[i] = recv_buffer[i];
			}
			break;
		}
		case 1: // Powerdown
		{
			if(AD5933_ctrl_powedown() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case 2: // SetClockDivider
		{
			clk_gen_set(recv_buffer[0]);
			send_buffer[0] = recv_buffer[0];
			break;
		}
		default: // Unknown request
		{
			return false;
		}
	}
	
	udd_g_ctrlreq.payload = send_buffer;
	udd_g_ctrlreq.payload_size = min(udd_g_ctrlreq.req.wLength, sizeof(send_buffer));
	
	ui_connection_state(false);
	return true;
}