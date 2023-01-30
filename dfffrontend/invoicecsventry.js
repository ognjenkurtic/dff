const btnParseCsv = document.getElementById("parse_csv");

btnParseCsv.addEventListener("click", async function (event) {
	event.preventDefault();

    const selectedFile = document.getElementById('invoices_csv_file').files[0];

    let reader = new FileReader();

    reader.onload = (function() {
        return function(e) {
            const parseResult = Papa.parse(e.target.result);
            
            console.log('Parsirani sadrzaj:' + parseResult);
            console.log('Broj linija: ' + parseResult.data.length);
            
            let i = 0;

            parseResult.data.foreach(async invoiceRow => {
                console.log(`Broj kolona u redu ${i + 1}: ${invoiceRow.length}`);

                if (invoiceRow.length != 7) {
                    console.log(`Broj kolona u redu nije 7.`);
                    return;
                }

                const generatedSignatures = await generateSignatures(invoiceRow[0], invoiceRow[1], invoiceRow[2], invoiceRow[3], invoiceRow[4], invoiceRow[5], invoiceRow[6]);
                console.log(`Generisani potpisi za red ${i + 1}: ${JSON.stringify(generatedSignatures)}`);
                i++;
            });
        };
    })(selectedFile);

    reader.readAsText(selectedFile);
});