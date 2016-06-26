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

#define RECEIVE_BUFFER_SIZE 4
#define SEND_BUFFER_SIZE 4

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
	
	switch((usb_command_t)bRequest)
	{
		case PING:
		{
			for(uint8_t i = 0; i < udd_g_ctrlreq.req.wLength; i++)
			{
				send_buffer[i] = recv_buffer[i];
			}
			break;
		}
		case INIT_CLK_GEN:
		{
			clk_gen_init();
			send_buffer[0] = 1;
			break;
		}
		case SET_CLK_GEN:
		{
			clk_gen_set(recv_buffer[0]);
			send_buffer[0] = recv_buffer[0];
			break;
		}
		case SWITCH_TO_CAL:
		{
			switch_to_calibrate();
			send_buffer[0] = 1;
			break;
		}
		case SWITCH_TO_MEAS:
		{
			switch_to_measure();
			send_buffer[0] = 1;
			break;
		}
		case AD5933_INIT:
		{
			AD5933_init();
			send_buffer[0] = 1;
			break;
		}
		case AD5933_REG_WRITE:
		{
			if(AD5933_reg_write(recv_buffer[0], recv_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_ADDR_POINTER:
		{
			if(AD5933_set_address_pointer(recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_BLOCK_WRITE:
		{
			if(AD5933_block_write(recv_buffer[0], &recv_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_REG_READ:
		{
			if(AD5933_reg_read(recv_buffer[0], &send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_BLOCK_READ:
		{
			if(AD5933_block_read(recv_buffer[0], &send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_INIT_WITH_START_FREQ:
		{
			if(AD5933_ctrl_init_with_start_freq() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_INCR_FREQ:
		{
			if(AD5933_ctrl_increment_freq() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_RPT_FREQ:
		{
			if(AD5933_ctrl_repeat_freq() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_START_MEAS_TEMP:
		{
			if(AD5933_ctrl_start_measure_temp() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_POWERDOWN:
		{
			if(AD5933_ctrl_powedown() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_STDBY:
		{
			if(AD5933_ctrl_standby() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SEL_VOUT_RANGE:
		{
			if(AD5933_ctrl_select_Vout_range(recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_PGA_GAIN:
		{
			if(AD5933_ctrl_set_PGA_gain(recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_RESET:
		{
			if(AD5933_ctrl_reset_part() == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SEL_MCLK:
		{
			if(AD5933_ctrl_select_mclk(recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_START_FREQ:
		{
			if(AD5933_set_start_freq(&recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_START_FREQ:
		{
			if(AD5933_get_start_freq(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_FREQ_INCR:
		{
			if(AD5933_set_freq_incr(&recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_FREQ_INCR:
		{
			if(AD5933_get_freq_incr(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_NUM_OF_INCR:
		{
			if(AD5933_set_num_of_incr(&recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_NUM_OF_INCR:
		{
			if(AD5933_get_num_of_incr(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_SET_NUM_OF_SETTL:
		{
			if(AD5933_set_num_of_settl(&recv_buffer[0]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_NUM_OF_SETTL:
		{
			if(AD5933_get_num_of_settl(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_TEMP:
		{
			if(AD5933_get_temp(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_REAL:
		{
			if(AD5933_get_real(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_GET_IMAG:
		{
			if(AD5933_get_imag(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_CHECK_VALID_TEMP:
		{
			if(AD5933_check_valid_temp(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_CHECK_SWEEP_COMPLETE:
		{
			if(AD5933_check_sweep_complete(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_CHECK_VALID_DATA:
		{
			if(AD5933_check_valid_data(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		case AD5933_MEASURE_TEMP:
		{
			if(AD5933_measure_temp(&send_buffer[1]) == true) send_buffer[0] = 1;
			else send_buffer[0] = 0;
			break;
		}
		default: // Error, unknown request.
		{
			return false;
		}
	}
	
	udd_g_ctrlreq.payload = send_buffer;
	udd_g_ctrlreq.payload_size = min(udd_g_ctrlreq.req.wLength, sizeof(send_buffer));
	
	ui_connection_state(false);
	return true;
}