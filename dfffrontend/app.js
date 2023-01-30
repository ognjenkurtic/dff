handleTabmenuClick('manual-tab');

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
