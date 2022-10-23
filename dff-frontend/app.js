const form = document.getElementById("invoice_data");

const btnGenSig = document.getElementById("generate_signatures");
const btnSendSig = document.getElementById("send_signatures");

btnGenSig.addEventListener("click", function (event) {
	event.preventDefault();
    console.log(form.elements["mat_broj_dobavljac"].value);
    console.log(form.elements["mat_broj_kupac"].value);
    console.log(form.elements["broj_fakture"].value);
    console.log(form.elements["datum_izdavanja"].value);
    console.log(form.elements["datum_valute"].value);
    console.log(form.elements["iznos"].value);
    // TODO: Generate sigs and populate aside
});

btnSendSig.addEventListener("click", function (event) {
	event.preventDefault();
    console.log("TODO: fire away");
});