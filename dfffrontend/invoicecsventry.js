const result_message_csv = document.getElementById("result_csv");
const result_signatures_csv = document.getElementById("results_and_signatures_csv");
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

            showSignaturesForCsvEntry(generatedSignatureSets);
            
            const response = await fetch(`${apiUrl}/api/Signatures/check`, {
                headers: fetchApiKeyAndPrepareHeaders(),
                method: 'POST',
                body: JSON.stringify(prepareReqBodyFromSignatureSets(generatedSignatureSets)),
            });

            processCsvUploadResponse(response);
        };
    })(selectedFile);

    reader.readAsText(selectedFile);
});

function showSignaturesForCsvEntry(singatureSets) {
    
    result_signatures_csv.textContent = ''
    let last_added_result_element;
    
    let i = 0;
    singatureSets.forEach(ss => {
        i++;

        let new_result_row_index = document.createElement('div');
        new_result_row_index.classList.add('csv_row_index');
        
        let new_result_row_content = document.createElement('div');
        new_result_row_content.classList.add('csv_row_result');

        new_result_row_index.textContent = `Red ${i}`;
        new_result_row_content.textContent = `s1:${ss.signature1} s2:${ss.signature2} s3:${ss.signature3} s4:${ss.signature4} s5:${ss.signature5}`

        if (i === 1) {
            result_signatures_csv.appendChild(new_result_row_index);
        } else {
            last_added_result_element.after(new_result_row_index);
        }

        new_result_row_index.after(new_result_row_content);

        last_added_result_element = new_result_row_content;

        // const new_result_row_index = csv_row_index[i-1].cloneNode();
        // const new_result_row_content = csv_row_result[i-1].cloneNode();



        // csv_row_result[i-1].after(new_result_row_index);
    });
}

async function processCsvUploadResponse(response, isStoreAction) {
    if (!response.ok) {
        processCsvUploadErrorResponse(response);
        return;
    }

    await processCsvUploadOkResponse(response, isStoreAction);
}

async function processCsvUploadErrorResponse(response) {
    result_message_csv.className = "result-error";

    switch (response.status) {
        case 401:
            result_message_csv.textContent = "Neuspešna autorizacija. Proverite API ključ.";
            break;
        case 400:
            result_message_csv.textContent = "Nevalidan zahtev. Proverite konzolu za više detalja.";
            break;
        default:
            result_message_csv.textContent = "Došlo je do greške. Proverite konzolu za više detalja.";
            break;
    }

    console.warn('Došlo je do greške. Full response: ', await response.json());
}

async function processCsvUploadOkResponse(response, isStoreAction) {
    const responseData = await response.json();
    console.log(JSON.stringify(responseData));

    // TODO: Check if this is legit response at all
    if (responseData.length === 0)
    {
        result_message_csv.textContent = "Faktura nije bila predmet faktoringa.";
        result_message_csv.className = "result-success";
        
        if (isStoreAction) {
            result_message_csv.textContent += " Potpisi su uspešno sačuvani u bazi.";
        }

        return;
    } 

    let hasDups = false;
    responseData.forEach(sigSetResponse => {
        if (sigSetResponse.hasDuplicates) {
            hasDups = true;
        }
    });

    if (hasDups) {
        result_message_csv.textContent = `Neka od faktura je bila predmet faktoringa. Proverite konzolu za detalje.`;
        result_message_csv.className = "result-error";
    } else {
        result_message_csv.textContent = "Nijedna faktura nije bila predmet faktoringa.";
        result_message_csv.className = "result-success";
    }
}