/**
 * \file
 *
 * \brief Main functions for USB Device vendor example
 *
 * Copyright (c) 2011 - 2014 Atmel Corporation. All rights reserved.
 *
 * \asf_license_start
 *
 * \page License
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 *
 * 3. The name of Atmel may not be used to endorse or promote products derived
 *    from this software without specific prior written permission.
 *
 * 4. This software may only be redistributed and used in connection with an
 *    Atmel microcontroller product.
 *
 * THIS SOFTWARE IS PROVIDED BY ATMEL "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT ARE
 * EXPRESSLY AND SPECIFICALLY DISCLAIMED. IN NO EVENT SHALL ATMEL BE LIABLE FOR
 * ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
 * ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *
 * \asf_license_stop
 *
 */

/*
 * Include header files for all drivers that have been imported from
 * Atmel Software Framework (ASF).
 */
#include <asf.h>
#include "conf_usb.h"
#include "ui.h"
#include "clock_divider.h"

static volatile bool main_b_vendor_enable = false;

/**
 * \name Buffer for communication
 */
//@{
//! Size of buffer used for communication
#define  MAIN_BUFFER_SIZE    1024
COMPILER_WORD_ALIGNED static uint8_t main_buffer[MAIN_BUFFER_SIZE];
//@}

void main_vendor_bulk_in_received(udd_ep_status_t status, iram_size_t nb_transfered, udd_ep_id_t ep);
void main_vendor_bulk_out_received(udd_ep_status_t status, iram_size_t nb_transfered, udd_ep_id_t ep);

/*! \brief Main function. Execution starts here.
 */
int main(void)
{
	irq_initialize_vectors();
	cpu_irq_enable();

	// Initialize the sleep manager
	sleepmgr_init();
	sysclk_init();
	board_init();
	ui_init();
	clk_gen_init();

	// Start USB stack to authorize VBus monitoring
	udc_start();

	// The main loop manages only the power mode
	// because the USB management is done by interrupt
	while (true)
	{
		sleepmgr_enter_sleep();
	}
}

void main_suspend_action(void)
{
	ui_powerdown();
}

void main_resume_action(void)
{
	ui_wakeup();
}

void main_sof_action(void)
{
	if (!main_b_vendor_enable)
	return;
	ui_process(udd_get_frame_number());
}

bool main_vendor_enable(void)
{
	main_b_vendor_enable = true;
	// Start data reception on OUT endpoints
	#if UDI_VENDOR_EPS_SIZE_BULK_FS
	main_vendor_bulk_in_received(UDD_EP_TRANSFER_OK, 0, 0);
	#endif
	return true;
}

void main_vendor_disable(void)
{
	main_b_vendor_enable = false;
}

bool main_setup_out_received(void)
{
	ui_connection_state(true);
	udd_g_ctrlreq.payload = main_buffer;
	udd_g_ctrlreq.payload_size = min(udd_g_ctrlreq.req.wLength, sizeof(main_buffer));
	return true;
}

bool main_setup_in_received(void)
{
	ui_connection_state(false);
	udd_g_ctrlreq.payload = main_buffer;
	udd_g_ctrlreq.payload_size = min(udd_g_ctrlreq.req.wLength, sizeof(main_buffer));
	return true;
}

#if UDI_VENDOR_EPS_SIZE_BULK_FS
void main_vendor_bulk_in_received(udd_ep_status_t status, iram_size_t nb_transfered, udd_ep_id_t ep)
{
	UNUSED(nb_transfered);
	UNUSED(ep);
	if (UDD_EP_TRANSFER_OK != status)
	{
		return; // Transfer aborted, then stop loopback
	}
	ui_connection_state(false);
	// Wait a full buffer
	udi_vendor_bulk_out_run(main_buffer, sizeof(main_buffer), main_vendor_bulk_out_received);
}

void main_vendor_bulk_out_received(udd_ep_status_t status, iram_size_t nb_transfered, udd_ep_id_t ep)
{
	UNUSED(ep);
	if (UDD_EP_TRANSFER_OK != status)
	{
		return; // Transfer aborted, then stop loopback
	}
	ui_connection_state(true);
	// Send on IN endpoint the data received on endpoint OUT
	udi_vendor_bulk_in_run(main_buffer, nb_transfered, main_vendor_bulk_in_received);
}
#endif