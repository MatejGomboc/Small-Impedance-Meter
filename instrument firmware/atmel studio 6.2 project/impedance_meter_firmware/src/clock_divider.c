/*
 * clock_divider.c
 *
 * Created: 11.6.2016 11:36:47
 *  Author: user
 */ 

#include "spi_master.h"
#include "config/conf_board.h"
#include "clock_divider.h"

struct spi_device SPI_CLK_GEN = {
	//! clk gen select id
	.id = CLK_GEN_CS
};

void clk_gen_init(void)
{
	spi_master_init(CLK_GEN_SPI);
	spi_master_setup_device(CLK_GEN_SPI, &SPI_CLK_GEN, SPI_MODE_0, CLK_GEN_SPI_BAUDRATE, 0);
	spi_enable(CLK_GEN_SPI);
	clk_gen_set(0x00);
}

void clk_gen_set(uint8_t setting)
{
	uint8_t data[1] = { setting };
	spi_select_device(CLK_GEN_SPI, &SPI_CLK_GEN);
	spi_write_packet(CLK_GEN_SPI, data, sizeof(data));
	spi_deselect_device(CLK_GEN_SPI, &SPI_CLK_GEN);
}