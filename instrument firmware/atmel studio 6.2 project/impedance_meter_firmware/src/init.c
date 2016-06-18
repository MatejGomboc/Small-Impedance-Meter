/**
 * \file
 *
 * \brief Board initialization
 *
 */

#include <asf.h>
#include <board.h>
#include <conf_board.h>

void board_init(void)
{
	ioport_configure_pin(LED_USB_RED, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(LED_USB_GREEN, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(LED_USB_BLUE, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);

	ioport_configure_pin(SWT_CAL_nMES, IOPORT_DIR_OUTPUT | IOPORT_INIT_LOW);
	
	ioport_configure_pin(CLK_GEN_MASTER_SCK,IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(CLK_GEN_MASTER_MOSI,IOPORT_DIR_OUTPUT | IOPORT_INIT_LOW);
	ioport_configure_pin(CLK_GEN_MASTER_MISO, IOPORT_DIR_INPUT);
	ioport_configure_pin(CLK_GEN_CS, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);	
}