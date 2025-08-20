// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Puedes agregar esto en site.js
document.querySelector('input[name="Dni"]').addEventListener('input', function (e) {
    this.value = this.value.replace(/\D/g, '');
});

document.querySelector('input[name="Telefono"]').addEventListener('input', function (e) {
    this.value = this.value.replace(/\D/g, '');
});