/*
* AD5933.c
*
* Created: 11.6.2016 15:41:41
*  Author: user
*/

#include "twi_master.h"
#include "config/conf_twim.h"
#include "AD5933.h"

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
		.length       = 1,                    // transfer data size (bytes)
		.no_wait      = false
	};

	// Perform write then check the result.
	uint_fast32_t timer = 0xFFFFFFFF;
	while (twi_master_write(TWI_AD5933, &packet_sent) != TWI_SUCCESS)
	{
		if (timer == 0) return false; // If no response and timeout.
		timer--;
	}
	
	return true; // Write successful.
}

bool AD5933_set_address_pointer(uint8_t reg_addr)
{
	twi_package_t packet_sent = {
		.addr[0]      = 0xB0,                 // AD5933 command to set the register address pointer
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = (void *)&reg_addr,    // transfer data, register address
		.length       = 1,                    // transfer data size (bytes)
		.no_wait      = false
	};

	// Perform write then check the result.
	uint_fast32_t timer = 0xFFFFFFFF;
	while (twi_master_write(TWI_AD5933, &packet_sent) != TWI_SUCCESS)
	{
		if (timer == 0) return false; // If no response and timeout.
		timer--;
	}
	
	return true; // Write successful.
}

bool AD5933_block_write(uint8_t num_bytes, uint8_t* reg_data)
{
	twi_package_t packet_sent = {
		.addr[0]      = 0xA0,                 // AD5933 command for block write
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = (void *)reg_data,     // transfer data source buffer
		.length       = num_bytes,            // transfer data size (bytes)
		.no_wait      = false
	};

	// Perform write then check the result.
	uint_fast32_t timer = 0xFFFFFFFF;
	while (twi_master_write(TWI_AD5933, &packet_sent) != TWI_SUCCESS)
	{
		if (timer == 0) return false; // If no response and timeout.
		timer--;
	}
	
	return true; // Write successful.
}

bool AD5933_reg_read(uint8_t reg_addr, uint8_t* reg_data)
{
	twi_package_t packet_received = {
		.addr[0]      = reg_addr,             // AD5933 register address
		.addr_length  = sizeof(uint8_t),      // AD5933 register address data size
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = reg_data,             // transfer data destination buffer
		.length       = 1,                    // transfer data size (bytes)
		.no_wait      = false
	};

	// Perform read then check the result.
	uint_fast32_t timer = 0xFFFFFFFF;
	while (twi_master_read(TWI_AD5933, &packet_received) != TWI_SUCCESS)
	{
		if (timer == 0) return false; // If no response and timeout.
		timer--;
	}
	
	return true; // Read successful.
}

bool AD5933_block_read(uint8_t num_bytes, uint8_t* reg_data)
{
	twi_package_t packet_received = {
		.addr[0]      = 0xA1,                 // AD5933 command for block read
		.addr_length  = sizeof(uint8_t),      // size of command
		.chip         = TWI_AD5933_BUS_ADDR,  // AD5933 bus address
		.buffer       = reg_data,             // transfer data destination buffer
		.length       = num_bytes,            // transfer data size (bytes)
		.no_wait      = false
	};

	// Perform read then check the result.
	uint_fast32_t timer = 0xFFFFFFFF;
	while (twi_master_read(TWI_AD5933, &packet_received) != TWI_SUCCESS)
	{
		if (timer == 0) return false; // If no response and timeout.
		timer--;
	}
	
	return true; // Read successful.
}