document.addEventListener('DOMContentLoaded', () => {

    document.querySelectorAll('[data-confirm]').forEach(el =>
        el.addEventListener('click', e => {
            if (!confirm(el.getAttribute('data-confirm') ||
                '¿Deseas continuar?')) {
                e.preventDefault();
            }
        })
    );

    const productSelect =
        document.querySelector('[data-product-details]');

    if (!productSelect) return;

    const url = productSelect.dataset.productDetails;

    console.log(url);

    const price = document.querySelector('[data-current-price]');
    const stock = document.querySelector('[data-current-stock]');
    const amount = document.querySelector('[data-total]');

    const quantity = document.querySelector('[name="Cantidad"]');
    const purchasePrice = document.querySelector('[name="Precio"]');
    const salePrice = document.querySelector('[name="PrecioVenta"]');

    const formatCurrency = value =>
        Number(value).toLocaleString('es-MX', {
            style: 'currency',
            currency: 'MXN'
        });

    const updateAmount = () => {

        const p = purchasePrice
            ? Number(purchasePrice.value || 0)
            : Number(salePrice?.value || 0);

        const q = Number(quantity?.value || 0);

        if (amount)
            amount.textContent = formatCurrency(p * q);
    };

    const refresh = async () => {

        if (!productSelect.value) {
            if (price) price.textContent = '—';
            if (stock) stock.textContent = '—';
            if (amount) amount.textContent = '$0.00';
            return;
        }

        try {
            const response = await fetch(
                `${url}?id=${encodeURIComponent(productSelect.value)}`


            );
            console.log(response);

            if (!response.ok)
                throw new Error('Error al consultar producto');

            const data = await response.json();

            console.log(data);
            if (price)
                price.textContent = formatCurrency(data.precioVenta);

            if (stock)
                stock.textContent = Number(data.existencia).toFixed(2);

            if (purchasePrice && !purchasePrice.value)
                purchasePrice.value =
                    Number(data.precioCompra).toFixed(2);

            if (salePrice && !salePrice.value)
                salePrice.value =
                    Number(data.precioVenta).toFixed(2);

            updateAmount();

        } catch {
            if (stock)
                stock.textContent = 'No disponible';
        }
    };

    productSelect.addEventListener('change', () => {
        if (purchasePrice) purchasePrice.value = '';
        if (salePrice) salePrice.value = '';
        refresh();
    });

    quantity?.addEventListener('input', updateAmount);
    purchasePrice?.addEventListener('input', updateAmount);
    salePrice?.addEventListener('input', updateAmount);

    refresh();
});

