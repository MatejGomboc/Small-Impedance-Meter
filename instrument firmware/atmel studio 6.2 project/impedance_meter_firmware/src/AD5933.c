/*
* AD5933.c
*
* Created: 11.6.2016 15:41:41
*  Author: user
*/

#include <status_codes.h>
#include "gpio.h"
#include "twi_master.h"
#include "config/conf_twim.h"
#include "AD5933.h"

void switch_to_calibrate(void)
{
	gpio_set_pin_high(SWT_CAL_nMES);
}

void switch_to_measure(void)
{
	gpio_set_pin_low(SWT_CAL_nMES);
}



void AD5933_init(void)
{
	// TWI master initialization options.

	twi_master_options_t opt;
	opt.speed = TWI_SPEED;
	opt.chip  = TWI_AD5933_BUS_ADDR;
	
	// Initialize the TWI master driver.

	twi_master_setup(TWI_AD5933, &opt);
}



bool AD5933_reg_write(uint8_t reg_addr, uint8_t reg_data)
{
	twi_package_t packet_sent = {
		.addr[0]      = reg_addr,             // AD5933 register address data MSB
		.addr_length  = sizeof(uint8_t),      // AD5933 register address data size
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = (void *)&reg_data,    // transfer data source buffer
		.length       = 1                     // transfer data size (bytes)
	};

	status_code_t status = OPERATION_IN_PROGRESS;
	// Perform write then check the result.
	do
	{
		status = twi_master_write(TWI_AD5933, &packet_sent);
	}
	while (status != TWI_SUCCESS);
	
	return true; // Write successful.
}

bool AD5933_set_address_pointer(uint8_t reg_addr)
{
	twi_package_t packet_sent = {
		.addr[0]      = 0xB0,                 // AD5933 command to set the register address pointer
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = (void *)&reg_addr,    // transfer data, register address
		.length       = 1                     // transfer data size (bytes)
	};

	status_code_t status = OPERATION_IN_PROGRESS;
	// Perform write then check the result.
	do
	{
		status = twi_master_write(TWI_AD5933, &packet_sent);
	}
	while (status != TWI_SUCCESS);
	
	return true; // Write successful.
}

bool AD5933_block_write(uint8_t num_bytes, uint8_t* reg_data)
{
	twi_package_t packet_sent = {
		.addr[0]      = 0xA0,                 // AD5933 command for block write
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = (void *)reg_data,     // transfer data source buffer
		.length       = num_bytes             // transfer data size (bytes)
	};

	status_code_t status = OPERATION_IN_PROGRESS;
	// Perform write then check the result.
	do
	{
		status = twi_master_write(TWI_AD5933, &packet_sent);
	}
	while (status != TWI_SUCCESS);
	
	return true; // Write successful.
}

bool AD5933_reg_read(uint8_t reg_addr, uint8_t* reg_data)
{
	twi_package_t packet_received = {
		.addr[0]      = reg_addr,             // AD5933 register address
		.addr_length  = sizeof(uint8_t),      // AD5933 register address data size
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = reg_data,             // transfer data destination buffer
		.length       = 1                     // transfer data size (bytes)
	};

	status_code_t status = OPERATION_IN_PROGRESS;
	// Perform read then check the result.
	do
	{
		status = twi_master_read(TWI_AD5933, &packet_received);
	}
	while (status != TWI_SUCCESS);
	
	return true; // Read successful.
}

bool AD5933_block_read(uint8_t num_bytes, uint8_t* reg_data)
{
	twi_package_t packet_received = {
		.addr[0]      = 0xA1,                 // AD5933 command for block read
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = reg_data,             // transfer data destination buffer
		.length       = num_bytes             // transfer data size (bytes)
	};

	status_code_t status = OPERATION_IN_PROGRESS;
	// Perform read then check the result.
	do
	{
		status = twi_master_read(TWI_AD5933, &packet_received);
	}
	while (status != TWI_SUCCESS);
	
	return true; // Read successful.
}



bool AD5933_ctrl_init_with_start_freq(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_INIT_WITH_START_FREQ << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_start_freq_sweep(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_START_FREQ_SWEEP << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_increment_freq(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_INCREMENT_FREQ << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_repeat_freq(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_REPEAT_FREQ << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);	
}

bool AD5933_ctrl_powedown(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_POWERDOWN << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_standby(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_STANDBY << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_reset_part(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_LOW, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_RESET << 4);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_LOW, ctrl_reg);
}