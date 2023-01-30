const invoiceForm = document.getElementById("invoice_data");

const btnChkDups = document.getElementById("check_duplicates");
const btnSendSig = document.getElementById("send_signatures");
const btnClearForm = document.getElementById("clear_form");

const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const pSignature4 = document.getElementById("potpis_4");
const result = document.getElementById("result");

btnChkDups.addEventListener("click", async function (event) {
	event.preventDefault();

    cleanupSignaturesAndResult();

    const [ 
        matBrojDob,
        matBrojKupac,
        brojFakture,
        datumIzdavanja,
        datumValute,
        iznos,
        sefId 
    ] = getInputsFromEntryForm();

    const generatedSignatures = await generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId);

    showSignaturesForManualEntry(generatedSignatures);

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

    const [ 
        matBrojDob,
        matBrojKupac,
        brojFakture,
        datumIzdavanja,
        datumValute,
        iznos,
        sefId 
    ] = getInputsFromEntryForm();

    const generatedSignatures = await generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId);

    showSignaturesForManualEntry(generatedSignatures);

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

function getInputsFromEntryForm() {
    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value;
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value;
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value; // 766e3a6b-1be5-48ac-bad4-eb12ceba540c

    return [matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId];
}

function showSignaturesForManualEntry(signatures) {
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