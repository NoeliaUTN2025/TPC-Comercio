// TPC-Comercio/AplicacionWebComercio/Scripts/site-ui.js
(function () {
    "use strict";

    function crearToastContainer() {
        var existente = document.getElementById("toastContainer");
        if (existente) return existente;

        var contenedor = document.createElement("div");
        contenedor.id = "toastContainer";
        contenedor.className = "toast-container position-fixed top-0 end-0 p-3";
        contenedor.style.zIndex = "1080";
        document.body.appendChild(contenedor);
        return contenedor;
    }

    function convertirAlertasEnToasts() {
        var contenido = document.getElementById("MainContent") || document.body;
        var alertas = contenido.querySelectorAll(".alert:not(.d-none)");

        if (alertas.length === 0) return;

        var contenedor = crearToastContainer();

        alertas.forEach(function (alerta) {
            var texto = alerta.textContent.trim();
            if (!texto) return;

            var esError = alerta.classList.contains("alert-danger");

            var toastEl = document.createElement("div");
            toastEl.className = "toast align-items-center border-0 " + (esError ? "text-bg-danger" : "text-bg-success");
            toastEl.setAttribute("role", "alert");
            toastEl.innerHTML =
                '<div class="d-flex">' +
                '<div class="toast-body">' + texto + '</div>' +
                '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>' +
                '</div>';

            contenedor.appendChild(toastEl);
            var toast = new bootstrap.Toast(toastEl, { delay: 4000 });
            toast.show();

            alerta.classList.add("d-none");
        });
    }

    function mostrarLoaderOverlay() {
        if (document.getElementById("loaderOverlay")) return;

        var overlay = document.createElement("div");
        overlay.id = "loaderOverlay";
        overlay.style.cssText =
            "position:fixed;inset:0;background:rgba(255,255,255,0.6);" +
            "display:flex;align-items:center;justify-content:center;z-index:1090;";
        overlay.innerHTML = '<div class="spinner-border text-primary" role="status"></div>';
        document.body.appendChild(overlay);
    }

    function habilitarLoaderEnPostback() {
        var formulario = document.querySelector("form");
        if (formulario) {
            formulario.addEventListener("submit", mostrarLoaderOverlay);
        }

        if (typeof window.__doPostBack === "function") {
            var doPostBackOriginal = window.__doPostBack;
            window.__doPostBack = function (eventTarget, eventArgument) {
                mostrarLoaderOverlay();
                doPostBackOriginal(eventTarget, eventArgument);
            };
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        convertirAlertasEnToasts();
        habilitarLoaderEnPostback();
    });
})();
