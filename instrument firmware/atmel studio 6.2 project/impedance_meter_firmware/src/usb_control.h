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
	INIT_CLK_GEN,
	SET_CLK_GEN,
	SWITCH_TO_CAL,
	SWITCH_TO_MEAS,
	AD5933_INIT,
	AD5933_REG_WRITE,
	AD5933_SET_ADDR_POINTER,
	AD5933_BLOCK_WRITE,
	AD5933_REG_READ,
	AD5933_BLOCK_READ,
	AD5933_INIT_WITH_START_FREQ,
	AD5933_INCR_FREQ,
	AD5933_RPT_FREQ,
	AD5933_START_MEAS_TEMP,
	AD5933_POWERDOWN,
	AD5933_STDBY,
	AD5933_SEL_VOUT_RANGE,
	AD5933_SET_PGA_GAIN,
	AD5933_RESET,
	AD5933_SEL_MCLK,
	AD5933_SET_START_FREQ,
	AD5933_GET_START_FREQ,
	AD5933_SET_FREQ_INCR,
	AD5933_GET_FREQ_INCR,
	AD5933_SET_NUM_OF_INCR,
	AD5933_GET_NUM_OF_INCR,
	AD5933_SET_NUM_OF_SETTL,
	AD5933_GET_NUM_OF_SETTL,
	AD5933_GET_TEMP,
	AD5933_GET_REAL,
	AD5933_GET_IMAG,
	AD5933_CHECK_VALID_TEMP,
	AD5933_CHECK_SWEEP_COMPLETE,
	AD5933_CHECK_VALID_DATA,
	AD5933_MEASURE_TEMP
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