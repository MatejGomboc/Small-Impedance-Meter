/*
 * usb_control.h
 *
 * Created: 25.6.2016 11:20:16
 *  Author: user
 */ 

#ifndef USB_CONTROL_H_
#define USB_CONTROL_H_

//!< List of all available commands
typedef enum
{
	PING = 0,
	INIT_CLK_GEN = 1,
	SET_CLK_GEN = 2,
	SWITCH_TO_CAL = 3,
	SWITCH_TO_MEAS = 4,
	AD5933_INIT = 5,
	AD5933_REG_WRITE = 6,
	AD5933_SET_ADDR_POINTER = 7,
	AD5933_BLOCK_WRITE = 8,
	AD5933_REG_READ = 9,
	AD5933_BLOCK_READ = 10,
	AD5933_INIT_WITH_START_FREQ = 11,
	AD5933_INCR_FREQ = 12,
	AD5933_RPT_FREQ = 13,
	AD5933_START_MEAS_TEMP = 14,
	AD5933_POWERDOWN = 15,
	AD5933_STDBY = 16,
	AD5933_SEL_VOUT_RANGE = 17,
	AD5933_SET_PGA_GAIN = 18,
	AD5933_RESET = 19,
	AD5933_SEL_MCLK = 20,
	AD5933_SET_START_FREQ = 21,
	AD5933_GET_START_FREQ = 22,
	AD5933_SET_FREQ_INCR = 23,
	AD5933_GET_FREQ_INCR = 24,
	AD5933_SET_NUM_OF_INCR = 25,
	AD5933_GET_NUM_OF_INCR = 26,
	AD5933_SET_NUM_OF_SETTL = 27,
	AD5933_GET_NUM_OF_SETTL = 28,
	AD5933_GET_TEMP = 29,
	AD5933_GET_REAL = 30,
	AD5933_GET_IMAG = 31,
	AD5933_CHECK_VALID_TEMP = 32,
	AD5933_CHECK_SWEEP_COMPLETE = 33,
	AD5933_CHECK_VALID_DATA = 34,
	AD5933_MEASURE_TEMP = 35
} usb_command_t;

/*! \brief Manage the reception of setup request OUT
 *
 * \retval true if request accepted
 */
bool main_setup_out_received(void);

/*! \brief Manage the reception of setup request IN
 *
 * \retval true if request accepted
 */
bool main_setup_in_received(void);

#endif /* USB_CONTROL_H_ */