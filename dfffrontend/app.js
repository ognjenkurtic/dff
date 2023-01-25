const authForm = document.getElementById("auth_data");
const invoiceForm = document.getElementById("invoice_data");

const btnChkDups = document.getElementById("check_duplicates");
const btnSendSig = document.getElementById("send_signatures");
const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const pSignature4 = document.getElementById("potpis_4");
const result = document.getElementById("result");

const btnClearForm = document.getElementById("clear_form");

btnChkDups.addEventListener("click", async function (event) {
	event.preventDefault();

    await generateSignatures();

    const response = await fetch('https://dff.finspot.rs/api/Signatures/check', {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(fetchSignaturesAndPrepareBody()),
    });

    await processResponse(response, false);
});

btnSendSig.addEventListener("click", async function (event) {
	event.preventDefault();

    await generateSignatures();

    const response = await fetch('https://dff.finspot.rs/api/Signatures/checkandstore', {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(fetchSignaturesAndPrepareBody()),
    });

    await processResponse(response, true);
});

btnClearForm.addEventListener("click", async function (event) {
	event.preventDefault();

    document.getElementById("invoice_data").reset();
})

async function generateSignatures() {
    cleanupSignaturesAndResult();

    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value;
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value;

    const signature1Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute + iznos;
    if (signature1Input !== undefined && signature1Input.length > 0) {
        const signature1 = await hashCode(signature1Input);
        pSignature1.textContent = signature1;
        pSignature1.className = '';
    }

    const signature2Input = matBrojDob + matBrojKupac + datumIzdavanja + datumValute + iznos;
    if (signature2Input !== undefined && signature2Input.length > 0) {
        const signature2 = await hashCode(signature2Input);
        pSignature2.textContent = signature2;
        pSignature2.className = '';
    }
    
    const signature3Input = matBrojDob + matBrojKupac + brojFakture + datumIzdavanja + datumValute;
    if (signature3Input !== undefined && signature3Input.length > 0) {
        const signature3 = await hashCode(signature3Input);
        pSignature3.textContent = signature3;
        pSignature3.className = '';
    }

    const signature4Input = sefId;
    if(signature4Input !== undefined && signature4Input.length > 0) {
        const signature4 = await hashCode(signature4Input);
        pSignature4.textContent = signature4;
        pSignature4.className = '';
    }
}

async function hashCode(str) {
    console.log(`Applying SHA-256 on the following string: ` + str)
    const textAsBuffer = new TextEncoder().encode(str);
    const hashBuffer = await window.crypto.subtle.digest('SHA-256', textAsBuffer);
    const hashArray = Array.from(new Uint8Array(hashBuffer))
    return hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
}

function fetchApiKeyAndPrepareHeaders() {
    const apiKey = authForm.elements["api_key"].value;

    const requestHeaders = new Headers();
    requestHeaders.append('Content-Type', 'application/json');
    requestHeaders.append('X-Factor-API-Key', apiKey);

    return requestHeaders;
}

function fetchSignaturesAndPrepareBody() {
    const signatureSet = {
        signature1: pSignature1.textContent,
        signature2: pSignature2.textContent,
        signature3: pSignature3.textContent,
        signature4: pSignature4.textContent,
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