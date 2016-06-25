/*
 * usb_control.h
 *
 * Created: 25.6.2016 11:20:16
 *  Author: user
 */ 

#ifndef USB_CONTROL_H_
#define USB_CONTROL_H_

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