const invoiceForm = document.getElementById("invoice_data");

const btnChkDups = document.getElementById("check_duplicates");
const btnSendSig = document.getElementById("send_signatures");
const btnClearForm = document.getElementById("clear_form");

const pSignature1 = document.getElementById("potpis_1");
const pSignature2 = document.getElementById("potpis_2");
const pSignature3 = document.getElementById("potpis_3");
const pSignature4 = document.getElementById("potpis_4");
const pSignature5 = document.getElementById("potpis_5");
const result = document.getElementById("result");
let hasDups = false;

// Regular expression to check if string is a valid UUID
const regexExp = /^[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}$/i;

btnChkDups.addEventListener("click", async function (event) {
    hasDups = false;
	event.preventDefault();

    const generatedSignatureSet = await performSignatureGenerationAndUpdateUI();

    const response = await fetch(`${apiUrl}/api/Signatures/check`, {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(prepareReqBodyFromSignatureSets([generatedSignatureSet])),
    });

    await processResponse(response, false);
});

btnSendSig.addEventListener("click", async function (event) {
    hasDups = false;
	event.preventDefault();

    const generatedSignatureSet = await performSignatureGenerationAndUpdateUI();

    const responseCheck = await fetch(`${apiUrl}/api/Signatures/check`, {
        headers: fetchApiKeyAndPrepareHeaders(),
    	method: 'POST',
        body: JSON.stringify(prepareReqBodyFromSignatureSets([generatedSignatureSet])),
    });

    await processResponse(responseCheck, false);

    if (hasDups) {
        const isConfirmed = confirm("Pronađeni su duplikati. Da li ste sigurni da želite da nastavite?");
        
        if (!isConfirmed)
        {
            return;
        }

        const responseStore = await fetch(`${apiUrl}/api/Signatures/checkandstore`, {
            headers: fetchApiKeyAndPrepareHeaders(),
            method: 'POST',
            body: JSON.stringify(prepareReqBodyFromSignatureSets([generatedSignatureSet])),
        });
    
        await processResponse(responseStore, true);
    }
});

btnClearForm.addEventListener("click", async function (event) {
	event.preventDefault();

    document.getElementById("invoice_data").reset();
})

async function performSignatureGenerationAndUpdateUI() {
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

    if (!matBrojDob && !sefId) {
        return;
    }

    const generatedSignatureSet = await generateSignatures(matBrojDob, matBrojKupac, brojFakture, datumIzdavanja, datumValute, iznos, sefId);

    showSignaturesForManualEntry(generatedSignatureSet);

    return generatedSignatureSet;
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
    pSignature5.textContent = '';
    pSignature5.className = '';
}

function getInputsFromEntryForm() {
    const matBrojDob = invoiceForm.elements["mat_broj_dobavljac"].value.trim();
    const matBrojKupac = invoiceForm.elements["mat_broj_kupac"].value.trim();
    const brojFakture = invoiceForm.elements["broj_fakture"].value;
    const datumIzdavanja = invoiceForm.elements["datum_izdavanja"].value;
    const datumValute = invoiceForm.elements["datum_valute"].value;
    const iznos = invoiceForm.elements["iznos"].value;
    const sefId = invoiceForm.elements["sef_id"].value; // 766e3a6b-1be5-48ac-bad4-eb12ceba540c

    const sefIdEntered = sefId !== undefined && sefId !== '';

    if (sefIdEntered) {
        if (!regexExp.test(sefId)) {
            alert('SEF ID mora biti validan GUID');
            return [ null ];
        }
    } else {
        if (matBrojDob.length != 8 || matBrojKupac.length != 8) {
            alert('Matični broj mora imati 8 brojeva');
            return [ null ];
        };

        if (!brojFakture) {
            alert('Broj fakture mora biti unet');
            return [ null ];
        }

        if (!datumIzdavanja) {
            alert('Datum izdavanja mora biti unet');
            return [ null ];
        }

        if (!datumValute) {
            alert('Datum valute mora biti unet');
            return [ null ];
        }

        if (!iznos) {
            alert('Iznos mora biti unet');
            return [ null ];
        }

        if(new Date(datumIzdavanja).getFullYear() < 2022) {
            alert('Godina izdavanja fakture mora biti nakon 2021.');
            return [ null ];
        }

        if(new Date(datumValute).getFullYear() > 2050) {
            alert('Godina valute fakture mora biti pre 2051.');
            return [ null ];
        }

        if(Date.parse(datumIzdavanja) > Date.parse(datumValute)) {
            alert('Datum izdavanja mora biti manji od datuma valute');
            return [ null ];
        }
    }

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

    if (signatures.signature5 !== undefined) {
        pSignature5.textContent = signatures.signature5;
        pSignature5.className = '';
    }
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
    if (responseData.length === 0) {
        result.className = "result-success";
        
        if (isStoreAction) {
            result.textContent += "Potpisi su uspešno sačuvani u bazi.";
        } else {
            result.textContent = "Faktura nije bila predmet faktoringa.";
        };

        hasDups = false;
        return;
    } 
    
    const signatureSetCheckResult = responseData[0];

    if (signatureSetCheckResult.hasDuplicates) {
        hasDups = true;

        let factorName = '';
        let factorEmail = '';

        const sig1Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 1);
        const sig2Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 2);
        const sig3Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 3);
        const sig4Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 4);
        const sig5Duplicate = signatureSetCheckResult.signatureDuplicateResponses.find(sdr => sdr.signatureType === 5);

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

        if (sig5Duplicate) {
            factorName = sig5Duplicate.factoringCompanyName;
            factorEmail = sig5Duplicate.email;
            pSignature5.className = "signature-alert";
        }

        result.textContent = `Faktura je bila predmet faktoringa kod ${factorName}. Kontakt email: ${factorEmail}`;
        result.className = "result-error";
    } else if (signatureSetCheckResult.ownDuplicate) {
        hasDups = true;
        result.textContent = `Ova faktura je duplikat sa fakturom koju ste vi već podneli`;
        result.className = "result-error";
    } else {
        hasDups = false;
        result.textContent = "Faktura nije bila predmet faktoringa.";
        result.className = "result-success";
    }

    if (isStoreAction) {
        result.textContent += " Potpisi su uspešno sačuvani u bazi.";
    }
}