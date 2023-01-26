const apiUrl = "https://dff.finspot.rs";

const authForm = document.getElementById("auth_data");
const invoiceForm = document.getElementById("invoice_data");

const btnChkDups = document.getElementById("check_duplicates");
const btnSendSig = document.getElementById("send_signatures");
const btnClearForm = document.getElementById("clear_form");

const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const pSignature4 = document.getElementById("potpis_4");
const result = document.getElementById("result");

const btnParseCsv = document.getElementById("parse_csv");

handleTabmenuClick('manual-tab');

function handleTabmenuClick(tabName) {
    let i, tabcontent, tablinks;
  
    // This is to clear the previous clicked content.
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
      tabcontent[i].style.display = "none";
    }
  
    // Set the tab to be "active".
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
      tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
  
    // Display the clicked tab and set it to active.
    document.getElementById(tabName).style.display = "block";

    // Set tab button to active.
    document.getElementById("btn-" + tabName).className += " active";
}

btnChkDups.addEventListener("click", async function (event) {
	event.preventDefault();

    cleanupSignaturesAndResult();

    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value;
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value;

    const generatedSignatures = await generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId);

    showSignatures(generatedSignatures);

    const response = await fetch(`${apiUrl}/api/Signatures/check`, {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(prepareReqBodyFromSignatures(pSignature1.textContent, pSignature2.textContent, pSignature3.textContent, pSignature4.textContent)),
    });

    await processResponse(response, false);
});

btnSendSig.addEventListener("click", async function (event) {
	event.preventDefault();

    cleanupSignaturesAndResult();

    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value;
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value;

    const generatedSignatures = await generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId);

    showSignatures(generatedSignatures);

    const response = await fetch(`${apiUrl}/api/Signatures/checkandstore`, {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(prepareReqBodyFromSignatures(pSignature1.textContent, pSignature2.textContent, pSignature3.textContent, pSignature4.textContent)),
    });

    await processResponse(response, true);
});

btnClearForm.addEventListener("click", async function (event) {
	event.preventDefault();

    document.getElementById("invoice_data").reset();
})

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
                const generatedSignatures = await generateSignatures(invoiceRow[0], invoiceRow[1], invoiceRow[2], invoiceRow[3], invoiceRow[4], invoiceRow[5], invoiceRow[6]);
                console.log(`Generisani potpisi za red ${i + 1}: ${JSON.stringify(generatedSignatures)}`);
                i++;
            });
        };
    })(selectedFile);

    reader.readAsText(selectedFile);
});

async function generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId) {
    let signature1;
    let signature2;
    let signature3;
    let signature4;

    const signature1Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos;
    if (signature1Input !== undefined && signature1Input.length > 0) {
        signature1 = await hashCode(signature1Input);
    }

    const signature2Input = matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos;
    if (signature2Input !== undefined && signature2Input.length > 0) {
        signature2 = await hashCode(signature2Input);
    }
    
    const signature3Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute;
    if (signature3Input !== undefined && signature3Input.length > 0) {
        signature3 = await hashCode(signature3Input);
    }

    const signature4Input = sefId;
    if(signature4Input !== undefined && signature4Input.length > 0) {
        signature4 = await hashCode(signature4Input);
    }

    return { signature1: signature1, signature2: signature2, signature3: signature3, signature4: signature4,  }
}

async function hashCode(str) {
    console.log(`Applying SHA-256 on the following string: ` + str)
    const textAsBuffer = new TextEncoder().encode(str);
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', textAsBuffer);
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

function showSignatures(signatures) {
    if (signatures.signature1 !== undefined) {
        pSignature1.textContent = signatures.signature1;
        pSignature1.className = '';
    }

    if (signatures.signature2 !== undefined) {
        pSignature2.textContent = signatures.signature2;
        pSignature2.className = '';
    }

    if (signatures.signature3 !== undefined) {
        pSignature3.textContent = signatures.signature3;
        pSignature3.className = '';
    }

    if (signatures.signature4 !== undefined) {
        pSignature4.textContent = signatures.signature4;
        pSignature4.className = '';
    }
}

function fetchApiKeyAndPrepareHeaders() {
    const apiKey = authForm.elements["api_key"].value;

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');
    requestHeaders.append('X-Factor-API-Key', apiKey);

    return requestHeaders;
}

function prepareReqBodyFromSignatures(sig1, sig2, sig3, sig4) {
    const signatureSet = {
        signature1: sig1,
        signature2: sig2,
        signature3: sig3,
        signature4: sig4,
    }

    const requestBody = {
        signaturesSets: [signatureSet]
    }

    return requestBody;
}

async function processResponse(response, isStoreAction) {
    if (!response.ok) {
        processErrorResponse(response);
        return;
    }

    await processOkResponse(response, isStoreAction);
}

async function processErrorResponse(response) {
    result.className = "result-error";

    switch (response.status) {
        case 401:
            result.textContent = "Neuspešna autorizacija. Proverite API ključ.";
            break;
        case 400:
            result.textContent = "Nevalidan zahtev. Proverite konzolu za više detalja.";
            break;
        default:
            result.textContent = "Došlo je do greške. Proverite konzolu za više detalja.";
            break;
    }

    console.warn('Došlo je do greške. Full response: ', await response.json());
}

async function processOkResponse(response, isStoreAction) {
    const responseData = await response.json();
    console.log(JSON.stringify(responseData));

    // TODO: Check if this is legit response at all
    if (responseData.length === 0)
    {
        result.textContent = "Faktura nije bila predmet faktoringa.";
        result.className = "result-success";
        
        if (isStoreAction) {
            result.textContent += " Potpisi su uspešno sačuvani u bazi.";
        }

        return;
    } 
    
    const signatureSetCheckResult = responseData[0];

    if (signatureSetCheckResult.hasDuplicates) {
        let factorName = '';
        let factorEmail = '';

        const sig1Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 1);
        const sig2Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 2);
        const sig3Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 3);
        const sig4Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 4);

        if (sig1Duplicate) {
            factorName = sig1Duplicate.factoringCompanyName;
            factorEmail = sig1Duplicate.email;
            pSignature1.className = "signature-alert";
        }

        if (sig2Duplicate) {
            factorName = sig2Duplicate.factoringCompanyName;
            factorEmail = sig2Duplicate.email;
            pSignature2.className = "signature-alert";
        }

        if (sig3Duplicate) {
            factorName = sig3Duplicate.factoringCompanyName;
            factorEmail = sig3Duplicate.email;
            pSignature3.className = "signature-alert";
        }

        if (sig4Duplicate) {
            factorName = sig4Duplicate.factoringCompanyName;
            factorEmail = sig4Duplicate.email;
            pSignature4.className = "signature-alert";
        }

        result.textContent = `Faktura je bila predmet faktoringa kod ${factorName}. Kontakt email: ${factorEmail}`;
        result.className = "result-error";
    } else {
        result.textContent = "Faktura nije bila predmet faktoringa.";
        result.className = "result-success";
        if (isStoreAction) {
            result.textContent += " Potpisi su uspešno sačuvani u bazi.";
        }
    }
}

function cleanupSignaturesAndResult() {
    result.textContent = '';
    pSignature1.textContent = '';
    pSignature1.className = '';
    pSignature2.textContent = '';
    pSignature2.className = '';
    pSignature3.textContent = '';
    pSignature3.className = '';
    pSignature4.textContent = '';
    pSignature4.className = '';
}