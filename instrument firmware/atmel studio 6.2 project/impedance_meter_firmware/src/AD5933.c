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

	status_code_t status;
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

bool AD5933_ctrl_start_measure_temp(void)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= (AD5933_CMD_MEASURE_TEMP << 4);
	
	if(!AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg)) return false;
	
	return true;
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
	
	ctrl_reg |= (AD5933_CMD_RESET << AD5933_CTRL_POS_RESET);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_LOW, ctrl_reg);
}

bool AD5933_ctrl_select_Vout_range(uint8_t setting)
{
	if(setting > 0x03) return false;
	
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	setting &= 0x03;
	
	ctrl_reg |= (setting << AD5933_CTRL_POS_VOUT_RANGE);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_set_PGA_gain(bool setting)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_HIGH, &ctrl_reg)) return false;
	
	ctrl_reg |= ((uint8_t)setting << AD5933_CTRL_POS_PGA_GAIN);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_HIGH, ctrl_reg);
}

bool AD5933_ctrl_select_mclk(bool setting)
{
	uint8_t ctrl_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_CTRL_LOW, &ctrl_reg)) return false;
	
	ctrl_reg |= ((uint8_t)setting << AD5933_CTRL_POS_MCLK_SEL);
	
	return AD5933_reg_write(AD5933_ADDR_CTRL_LOW, ctrl_reg);
}



bool AD5933_get_start_freq(uint8_t* freq_data)
{	
	if(!AD5933_set_address_pointer(AD5933_ADDR_FREQ_HIGH)) return false;
	
	if(!AD5933_block_read(3, freq_data)) return false;
	
	return true;
}

bool AD5933_set_start_freq(uint8_t* freq_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_FREQ_HIGH)) return false;
	
	if(!AD5933_block_write(3, freq_data)) return false;
	
	return true;
}


bool AD5933_get_freq_incr(uint8_t* freq_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_FREQ_INC_HIGH)) return false;
	
	if(!AD5933_block_read(3, freq_data)) return false;
	
	return true;
}

bool AD5933_set_freq_incr(uint8_t* freq_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_FREQ_INC_HIGH)) return false;
	
	if(!AD5933_block_write(3, freq_data)) return false;
	
	return true;
}

bool AD5933_get_num_of_incr(uint8_t* num_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_NUM_INC_HIGH)) return false;
	
	if(!AD5933_block_read(2, num_data)) return false;
	
	return true;
}

bool AD5933_set_num_of_incr(uint8_t* num_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_NUM_INC_HIGH)) return false;
	
	if(!AD5933_block_write(2, num_data)) return false;
	
	return true;
}


bool AD5933_get_num_of_settl(uint8_t* settl_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_NUM_SETTLE_HIGH)) return false;
	
	if(!AD5933_block_read(2, settl_data)) return false;
	
	return true;
}

bool AD5933_set_num_of_settl(uint8_t* settl_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_NUM_SETTLE_HIGH)) return false;
	
	if(!AD5933_block_write(2, settl_data)) return false;
	
	return true;
}


bool AD5933_get_temp(uint8_t* temp_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_TEMP_HIGH)) return false;
	
	if(!AD5933_block_read(2, temp_data)) return false;
	
	return true;
}

bool AD5933_get_real(uint8_t* real_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_REAL_HIGH)) return false;
	
	if(!AD5933_block_read(2, real_data)) return false;
	
	return true;
}

bool AD5933_get_imag(uint8_t* imag_data)
{
	if(!AD5933_set_address_pointer(AD5933_ADDR_IMAG_HIGH)) return false;
	
	if(!AD5933_block_read(2, imag_data)) return false;
	
	return true;
}


bool AD5933_check_valid_temp(bool* status)
{
	uint8_t status_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_STATUS, &status_reg)) return false;
	
	if(((status_reg >> AD5933_STAT_POS_TEMP) & 0x01) == 0x01) *status = true;
	else *status = false;
	
	return true;
}

bool AD5933_check_sweep_complete(bool* status)
{
	uint8_t status_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_STATUS, &status_reg)) return false;
	
	if(((status_reg >> AD5933_STAT_POS_FREQ_SWEEP_COMPLETE) & 0x01) == 0x01) *status = true;
	else *status = false;
	
	return true;
}

bool AD5933_check_valid_data(bool* status)
{
	uint8_t status_reg = 0x00;
	
	if(!AD5933_reg_read(AD5933_ADDR_STATUS, &status_reg)) return false;
	
	if(((status_reg >> AD5933_STAT_POS_RE_IM_DATA) & 0x01) == 0x01) *status = true;
	else *status = false;
	
	return true;
}


bool AD5933_measure_temp(uint8_t* temp_data)
{	
	if(!AD5933_ctrl_start_measure_temp()) return false;
	
	bool status = false;
	while(status == false)
	{
		if(!AD5933_check_valid_temp(&status)) return false;
	}
	
	if(!AD5933_get_temp(temp_data)) return false;
	
	return true;
}