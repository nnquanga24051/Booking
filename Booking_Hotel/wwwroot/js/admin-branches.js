function openAddBranchModal() {
    document.getElementById("branchId").value = "";
    document.getElementById("branchName").value = "";
    document.getElementById("branchDescription").value = "";
    document.getElementById("modalTitle").innerText = "➕ Thêm Chi Nhánh Mới";
    document.getElementById("branchModal").classList.add("show");
}

function closeBranchModal() {
    document.getElementById("branchModal").classList.remove("show");
}


    function saveBranch() {
        const name = document.getElementById("branchName").value.trim();
        if (!name) {
            alert("Tên chi nhánh không được để trống");
            return;
        }

        const id = document.getElementById("branchId").value;
        const data = {
            branchId: id ? parseInt(id) : 0,
            branchName: name,
            address: document.getElementById("branchDescription").value
        };

        const url = id ? "/Admin/UpdateBranch" : "/Admin/CreateBranch";

        fetch(url, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data)
        })
            .then(async res => {
                if (!res.ok) {
                    const text = await res.text();
                    throw text;
                }
                location.reload();
            })
            .catch(err => alert(err));
    }


function editBranch(id) {
    fetch(`/Admin/GetBranch?id=${id}`)
        .then(res => res.json())
        .then(data => {
            document.getElementById("branchId").value = data.branchId;
            document.getElementById("branchName").value = data.branchName;
            document.getElementById("branchDescription").value = data.address;
            document.getElementById("modalTitle").innerText = "✏️ Cập Nhật Chi Nhánh";
            document.getElementById("branchModal").classList.add("show");
        });
}

function deleteBranch(id) {
    if (!confirm("Bạn có chắc muốn xóa chi nhánh này?")) return;

    fetch(`/Admin/DeleteBranch?id=${id}`, { method: "POST" })
        .then(res => {
            if (!res.ok) throw "Xóa thất bại";
            location.reload();
        })
        .catch(err => alert(err));
}
