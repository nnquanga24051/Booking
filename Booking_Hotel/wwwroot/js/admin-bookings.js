const searchInput = document.getElementById("searchInput");
const statusFilter = document.getElementById("statusFilter");
const rows = document.querySelectorAll("#bookingTable tbody tr");

function filterBookings() {
    const keyword = searchInput.value.toLowerCase();
    const status = statusFilter.value.toLowerCase();

    rows.forEach(row => {
        const customer = row.children[1].innerText.toLowerCase();
        const room = row.children[2].innerText.toLowerCase();
        const bookingStatus = row.children[5].innerText.toLowerCase();

        let match = true;

        if (keyword && !customer.includes(keyword) && !room.includes(keyword))
            match = false;

        if (status && bookingStatus !== status)
            match = false;

        row.style.display = match ? "" : "none";
    });
}

searchInput.addEventListener("input", filterBookings);
statusFilter.addEventListener("change", filterBookings);

function updateStatus(id, status) {
    if (!confirm("Cập nhật trạng thái booking?")) return;

    fetch(`/Admin/UpdateBookingStatus?id=${id}&status=${status}`, {
        method: "POST"
    }).then(() => location.reload());
}

function deleteBooking(id) {
    if (!confirm("Xoá booking này?")) return;

    fetch(`/Admin/DeleteBooking?id=${id}`, {
        method: "POST"
    }).then(() => location.reload());
}
