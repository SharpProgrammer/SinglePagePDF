
## Purpose: ##

This project will demonstrate the creation of a fully working PDF file in code. By isolating the code responsible for generating the text inside a small routinue of the application an average programmer should be able to make simple modifications and generate a meaningfull PDF of thier own. No large learning curve, external DLL or complex library is required.


#### The Breakdown of the Projects Methods ####



- Output_To_Screen
- button1_Click
- Form1_Load
- Generate_PDF
- Add_Header_To_List
- Add_Catalog_To_List
- Add_Root_Page_To_List
- Add_First_Page_To_List
- Add_Font_Definition_To_List
- Add_Font_Definition_To_List2
- Add_First_Page_Text_Layout_To_List
- Add_Cross_Reference_Table_To_List
- Add_Trailer_To_List
- Add_End_Of_File
- Get_List_Of_Bytes_Total_Count
- Remove_Old_File_If_Present
- Physically_Write_Out_The_PDF



The **Output_To_Screen**, **button1_Click** and **Form1_Load** just support the basic windows form style of application so my comments will be focused on the methods which deal with the actual creation of the PDF file.


The **Generate_PDF** method takes a single input argument for the name of the file to create and then proceeds to call all the methods required to build up the PDF. These supporting methods all work by adding a byte array for the relevant section of the PDF file.

The **Add_Header_To_List** method like all the other supporting methods adds a byte array to a list of byte arrays passed in and out of each supporting method. 

Each PDF file has a catalog of the objects which make up the PDF. So in the
 **Add_Catalog_To_List** method this essential piece is added.

Each PDF starts with a root page so this is added in the **Add_Root_Page_To_List** method.

Many PDF files have multiple pages but in this case we will have only one. The  **Add_First_Page_To_List** adds this to the list of byte arrays. In a multiple page PDF document this type of method would be added multiple times.

The next two methods **Add_Font_Definition_To_List** and **Add_Font_Definition_To_List2** both work to determine the type of font used in the PDF File. While I have also played around with using a single font definition because that solution introduces an artifact I cant fully explain I have decided to stick with this pattern for now.

In the **Add_First_Page_Text_Layout_To_List** method most of the work of drawing text to be displayed is done. Changes anyone might make to generate a different PDF file will be done here.

Each PDF file also contains a Cross Reference Table so the method  **Add_Cross_Reference_Table_To_List** performs this vital function. Basically this table contains pointers to all the other objects in the PDF file.

Lastly we use the these two methods **Add_Trailer_To_List** and **Add_End_Of_File** to finish off the last few objects needed to make a complete PDF file.

The supporting method **Get_List_Of_Bytes_Total_Count** provides the count of the number of bytes in a specific byte array

The **Remove_Old_File_If_Present** method is just a supporting method used to ensure we generate a new file each time the program is run. Its more for a debugging sort of functionality supporting the way the windows form application generates a new file each time the button is pressed.

The **Physically_Write_Out_The_PDF** method takes the list of byte arrays and generates the physical file. Up to this point the built up PDF exists only in memory.

