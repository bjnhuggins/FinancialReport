# FinancialReport
Create financial reports from csv files

This program will trigger the processing of csv files at 6am and 9pm. It will pick up csv files in the pending folder, process them and
move the files to processed. Each processed file will have a report created to summarise the information in the transaction file and 
placed in the reports folder.

#Running the program

Pass in the location of the working directory eg c:\dev\FinancialReport as a parameter. This will create the processed, pending and reports 
directories if they do not already exist. The application will pick up all files in the pending folder at 6am and 9pm for processing.

#Improvements

Future improvements would be:
- Create a new thread to process each file with a maximum number of threads to improve processing.
- Add a logging file to write and errors to.
- Change the data type to decimal instead of double to avoid rounding errors.

#Assumptions

- All the files that need to be read will be available at 6am and 9pm. If they will be added within an hour range then another 
improvement of adding a watcher to the pending folder for a time period should be added.
- All file names added to the folder will be in the format finance_customer_transactions-${datetime}.csv, and the modified date of the file
will match the file names date time.
