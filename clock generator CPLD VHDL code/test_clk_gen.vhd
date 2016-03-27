LIBRARY ieee;
USE ieee.std_logic_1164.ALL;
 
ENTITY test_clk_gen IS
END test_clk_gen;
 
ARCHITECTURE behavior OF test_clk_gen IS 
 
    -- Component Declaration for the Unit Under Test (UUT)
 
    COMPONENT clk_gen
    PORT(
         clk : IN  std_logic;
         sck : IN  std_logic;
         data : IN  std_logic;
         cs_n : IN  std_logic;
         output : OUT  std_logic
        );
    END COMPONENT;
    

   --Inputs
   signal clk : std_logic := '0';
   signal sck : std_logic := '0';
   signal data : std_logic := '0';
   signal cs_n : std_logic := '0';

 	--Outputs
   signal output : std_logic;

   -- Clock period definitions
   constant clk_period : time := 62.5 ns;
   constant sck_period : time := 1 us;
 
BEGIN
 
	-- Instantiate the Unit Under Test (UUT)
   uut: clk_gen PORT MAP (
          clk => clk,
          sck => sck,
          data => data,
          cs_n => cs_n,
          output => output
        );

   -- Clock process definitions
   clk_process :process
   begin
		clk <= '0';
		wait for clk_period/2;
		clk <= '1';
		wait for clk_period/2;
   end process;
 

   -- Stimulus process
   stim_proc: process
   begin		
      wait for sck_period*100;
		
		data <= '0';
		cs_n <= '1';
		sck <= '0';
		
		wait for sck_period*100;
		
		cs_n <= '0';
		
		for i in 0 to 6 loop
			sck <= '0';
			wait for sck_period/2;
			sck <= '1';
			wait for sck_period/2;
		end loop;
		
		data <= '1';
		
		sck <= '0';
		wait for sck_period/2;
		sck <= '1';
		wait for sck_period/2;
		
		cs_n <= '1';
		
		wait for sck_period*100;
		
		data <= '1';
		cs_n <= '0';
		
		for i in 0 to 6 loop
			sck <= '0';
			wait for sck_period/2;
			sck <= '1';
			wait for sck_period/2;
		end loop;
		
		data <= '0';
		
		sck <= '0';
		wait for sck_period/2;
		sck <= '1';
		wait for sck_period/2;
		
		cs_n <= '1';

      wait;
   end process;

END;
