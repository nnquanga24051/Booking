function openAddRoomModal() {
    document.getElementById("roomForm").reset();
    document.getElementById("roomId").value = "";
    document.getElementById("modalTitle").innerText = "➕ Thêm Phòng Mới";
    document.getElementById("roomModal").style.display = "block";
}

function closeRoomModal() {
    document.getElementById("roomModal").style.display = "none";
}

//function saveRoom() {
//    const data = {
//        roomId: document.getElementById("roomId").value || 0,
//        roomName: document.getElementById("roomName").value,
//        branchId: document.getElementById("branchId").value,
//        roomType: document.getElementById("roomType").value,
//        price: document.getElementById("price").value,
//        capacity: document.getElementById("capacity").value,
//        status: document.getElementById("status").value,
//        imageUrl: document.getElementById("imageUrl").value
//    };

//    fetch("/Admin/SaveRoom", {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify(data)
//    })
//        .then(res => {
//            if (!res.ok) throw new Error();
//            location.reload();
//        })
//        .catch(() => alert("Lỗi lưu phòng"));
//}
function saveRoom() {

    // clear lỗi cũ
    document.querySelectorAll(".form-error").forEach(e => e.classList.remove("form-error"));
    document.querySelectorAll(".error-text").forEach(e => e.remove());

    let isValid = true;

    function showError(id, message) {
        const input = document.getElementById(id);
        input.classList.add("form-error");

        const err = document.createElement("div");
        err.className = "error-text";
        err.innerText = message;

        input.parentElement.appendChild(err);
        isValid = false;
    }

    const roomName = document.getElementById("roomName").value.trim();
    const branchId = document.getElementById("branchId").value;
    const price = document.getElementById("price").value;
    const capacity = document.getElementById("capacity").value;

    if (!roomName)
        showError("roomName", "Tên phòng không được để trống");

    if (!branchId)
        showError("branchId", "Vui lòng chọn chi nhánh");

    if (!price || price <= 0)
        showError("price", "Giá phải lớn hơn 0");

    if (!capacity || capacity <= 0)
        showError("capacity", "Sức chứa phải lớn hơn 0");

    if (!isValid) return;

    // ===== GIỮ NGUYÊN PHẦN CŨ =====
    const data = {
        roomId: parseInt(document.getElementById("roomId").value) || 0,
        roomName,
        branchId: parseInt(branchId),
        roomType: document.getElementById("roomType").value,
        price: parseFloat(price),
        capacity: parseInt(capacity),
        status: document.getElementById("status").value,
        imageUrl: document.getElementById("imageUrl").value
    };

    fetch("/Admin/SaveRoom", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(res => {
            if (!res.ok) throw new Error();
            location.reload();
        })
        .catch(() => alert("❌ Lỗi lưu phòng"));
}


function editRoom(id) {
    fetch(`/Admin/GetRoom?id=${id}`)
        .then(res => res.json())
        .then(r => {
            document.getElementById("roomId").value = r.roomId;
            document.getElementById("roomName").value = r.roomName;
            document.getElementById("branchId").value = r.branchId;
            document.getElementById("roomType").value = r.roomType;
            document.getElementById("price").value = r.price;
            document.getElementById("capacity").value = r.capacity;
            document.getElementById("status").value = r.status;
            document.getElementById("imageUrl").value = r.imageUrl;

            document.getElementById("modalTitle").innerText = "✏️ Cập Nhật Phòng";
            document.getElementById("roomModal").style.display = "block";
        });
}

function deleteRoom(id) {
    if (!confirm("Bạn chắc chắn muốn xóa phòng này?")) return;

    fetch(`/Admin/DeleteRoom?id=${id}`, { method: "POST" })
        .then(res => {
            if (!res.ok) throw new Error();
            location.reload();
        })
        .catch(() => alert("Không thể xóa phòng"));
}
document.getElementById("searchInput").addEventListener("keyup", () => {
    const keyword = document.getElementById("searchInput").value;
    window.location.href = `/Admin/Rooms?keyword=${keyword}`;
});
const searchInput = document.getElementById("searchInput");
const branchFilter = document.getElementById("branchFilter");
const typeFilter = document.getElementById("typeFilter");
const statusFilter = document.getElementById("statusFilter");

const rows = document.querySelectorAll("#roomsTable tbody tr");

function filterRooms() {
    const keyword = searchInput.value.toLowerCase();
    const branch = branchFilter.value;
    const type = typeFilter.value.toLowerCase();
    const status = statusFilter.value.toLowerCase();

    rows.forEach(row => {
        const roomName = row.children[3].innerText.toLowerCase();
        const branchName = row.children[4].innerText;
        const roomType = row.children[5].innerText.toLowerCase();
        const roomStatus = row.children[7].innerText.toLowerCase();

        let isMatch = true;

        if (keyword && !roomName.includes(keyword)) isMatch = false;
        if (branch && !branchName.includes(branch)) isMatch = false;
        if (type && !roomType.includes(type)) isMatch = false;
        if (status && !roomStatus.includes(status)) isMatch = false;

        row.style.display = isMatch ? "" : "none";
    });
}

// 🔥 LIVE FILTER
searchInput.addEventListener("input", filterRooms);
branchFilter.addEventListener("change", filterRooms);
typeFilter.addEventListener("change", filterRooms);
statusFilter.addEventListener("change", filterRooms);
