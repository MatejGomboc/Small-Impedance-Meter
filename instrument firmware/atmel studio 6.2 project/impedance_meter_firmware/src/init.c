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
	ioport_configure_pin(LED0_GPIO, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(LED1_GPIO, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(LED2_GPIO, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);

	ioport_configure_pin(CLK_GEN_MASTER_SCK,IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(CLK_GEN_MASTER_MOSI,IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);
	ioport_configure_pin(CLK_GEN_MASTER_MISO, IOPORT_DIR_INPUT);
	ioport_configure_pin(CLK_GEN_CS, IOPORT_DIR_OUTPUT | IOPORT_INIT_HIGH);	
}