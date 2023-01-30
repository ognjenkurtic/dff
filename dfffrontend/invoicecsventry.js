const btnParseCsv = document.getElementById("parse_csv");

btnParseCsv.addEventListener("click", async function (event) {
	event.preventDefault();

    const selectedFile = document.getElementById('invoices_csv_file').files[0];

    let reader = new FileReader();

    reader.onload = (function() {
        return async function(e) {
            const parseResult = Papa.parse(e.target.result);
            
            console.log('Parsirani sadrzaj:' + parseResult);
            console.log('Broj linija: ' + parseResult.data.length);
            
            let i = 0;

            const generatedSignatureSets = [];

            for (const invoiceRow of parseResult.data) {
                console.log(`Broj kolona u redu ${i + 1}: ${invoiceRow.length}`);

                if (invoiceRow.length != 7) {
                    console.log(`Broj kolona u redu nije 7.`);
                    i++;
                    continue;
                }

                const generatedSignatureSet = await generateSignatures(invoiceRow[0], invoiceRow[1], invoiceRow[2], invoiceRow[3], invoiceRow[4], invoiceRow[5], invoiceRow[6]);
                console.log(`Generisani potpisi za red ${i + 1}: ${JSON.stringify(generatedSignatureSet)}`);
                generatedSignatureSets.push(generatedSignatureSet);
                i++;
            }

            // showSignaturesForCsvEntry(generatedSignatureSets);
            
            const response = await fetch(`${apiUrl}/api/Signatures/check`, {
                headers: fetchApiKeyAndPrepareHeaders(),
                method: 'POST',
                body: JSON.stringify(prepareReqBodyFromSignatureSets(generatedSignatureSets)),
            });

            console.log(await response.json());
        };
    })(selectedFile);

    reader.readAsText(selectedFile);
});