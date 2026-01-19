//function openAddRoomModal() {
//    document.getElementById("roomForm").reset();
//    document.getElementById("roomId").value = "";
//    document.getElementById("modalTitle").innerText = "➕ Thêm Phòng Mới";
//    document.getElementById("roomModal").style.display = "block";
//}

//function closeRoomModal() {
//    document.getElementById("roomModal").style.display = "none";
//}

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

//function editRoom(id) {
//    fetch(`/Admin/GetRoom?id=${id}`)
//        .then(res => res.json())
//        .then(r => {
//            document.getElementById("roomId").value = r.roomId;
//            document.getElementById("roomName").value = r.roomName;
//            document.getElementById("branchId").value = r.branchId;
//            document.getElementById("roomType").value = r.roomType;
//            document.getElementById("price").value = r.price;
//            document.getElementById("capacity").value = r.capacity;
//            document.getElementById("status").value = r.status;
//            document.getElementById("imageUrl").value = r.imageUrl;

//            document.getElementById("modalTitle").innerText = "✏️ Cập Nhật Phòng";
//            document.getElementById("roomModal").style.display = "block";
//        });
//}

//function deleteRoom(id) {
//    if (!confirm("Bạn chắc chắn muốn xóa phòng này?")) return;

//    fetch(`/Admin/DeleteRoom?id=${id}`, { method: "POST" })
//        .then(res => {
//            if (!res.ok) throw new Error();
//            location.reload();
//        })
//        .catch(() => alert("Không thể xóa phòng"));
//}
//document.getElementById("searchInput").addEventListener("keyup", () => {
//    const keyword = document.getElementById("searchInput").value;
//    window.location.href = `/Admin/Rooms?keyword=${keyword}`;
//});
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
