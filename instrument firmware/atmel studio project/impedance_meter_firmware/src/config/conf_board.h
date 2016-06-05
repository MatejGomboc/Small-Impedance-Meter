/**
 * \file
 *
 * \brief Board configuration
 *
 * Copyright (c) 2011 Atmel Corporation. All rights reserved.
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

#ifndef CONF_BOARD_H_INCLUDED
#define CONF_BOARD_H_INCLUDED

/*! \name GPIO Connections of LEDs. To use these defines, connect the STK600
 * PORTE connector to the LEDs connector.
 */
//! @{
# define LED0_GPIO   IOPORT_CREATE_PIN(PORTA,4) // RED
# define LED1_GPIO   IOPORT_CREATE_PIN(PORTA,3) // GREEN
# define LED2_GPIO   IOPORT_CREATE_PIN(PORTA,2) // BLUE
//! @}

/*! \name SPI Connections of the clock generator. To use these defines,
 * connect :
 * - PC4 to /CS
 * - PC7 to SCK
 * - PC6 to SO
 * - PC5 to SI
 */
//! @{
#define CLK_GEN_SPI           &SPIC
#define CLK_GEN_CS            IOPORT_CREATE_PIN(PORTC,4)  // CS as output
#define CLK_GEN_MASTER_SS     IOPORT_CREATE_PIN(PORTC,4)  // SS as output and overlayed with CS
#define CLK_GEN_MASTER_SCK    IOPORT_CREATE_PIN(PORTC,7)  // SCK as output
#define CLK_GEN_MASTER_MOSI   IOPORT_CREATE_PIN(PORTC,5)  // MOSI as output
#define CLK_GEN_MASTER_MISO   IOPORT_CREATE_PIN(PORTC,6)  // MISO as input
#define CLK_GEN_SPI_CLK_MASK  SYSCLK_PORT_C

//! @}

/*! \name External oscillator
 */
//@{
#define BOARD_XOSC_HZ          16000000
#define BOARD_XOSC_TYPE        XOSC_TYPE_XTAL //XOSC_TYPE_EXTERNAL
#define BOARD_XOSC_STARTUP_US  50
//@}

#endif /* CONF_BOARD_H_INCLUDED */