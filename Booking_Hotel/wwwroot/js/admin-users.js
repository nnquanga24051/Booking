console.log("ADMIN USERS JS LOADED");

function openAddUserModal() {
    document.getElementById("modalTitle").innerText = "Thêm Người Dùng";
    document.getElementById("userId").value = "";
    document.getElementById("fullName").value = "";
    document.getElementById("email").value = "";
    document.getElementById("password").value = "";
    document.getElementById("role").value = "User";
    document.getElementById("formError").style.display = "none";

    document.getElementById("userModal").classList.add("show");
}

function closeUserModal() {
    document.getElementById("userModal").classList.remove("show");
}

function editUser(id) {
    fetch(`/Admin/GetUser?id=${id}`)
        .then(res => res.json())
        .then(data => {
            document.getElementById("modalTitle").innerText = "Cập Nhật Người Dùng";
            document.getElementById("userId").value = data.userId;
            document.getElementById("fullName").value = data.fullName;
            document.getElementById("email").value = data.email;
            document.getElementById("password").value = "";
            document.getElementById("role").value = data.role;
            document.getElementById("formError").style.display = "none";

            document.getElementById("userModal").classList.add("show");
        })
        .catch(err => console.error(err));
}
function saveUser() {

    const id = document.getElementById("userId").value;
    const fullName = document.getElementById("fullName").value.trim();
    const email = document.getElementById("email").value.trim();
    const password = document.getElementById("password").value;
    const role = document.getElementById("role").value;

    const messageBox = document.getElementById("formMessage");
    messageBox.style.display = "none";
    messageBox.className = "form-message";

    // ===== VALIDATE CLIENT =====
    if (!fullName || !email || (!id && !password)) {
        messageBox.innerText = "Vui lòng nhập đầy đủ thông tin";
        messageBox.classList.add("error");
        messageBox.style.display = "block";
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        messageBox.innerText = "Email không đúng định dạng";
        messageBox.classList.add("error");
        messageBox.style.display = "block";
        return;
    }

    const model = {
        userId: id ? parseInt(id) : 0,
        fullName,
        email,
        password,
        role
    };

    const url = id ? "/Admin/UpdateUser" : "/Admin/AddUser";

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(model)
    })
        .then(async res => {
            const text = await res.text();
            if (!res.ok) throw new Error(text);
            return text;
        })
        .then(msg => {
            messageBox.innerText = msg;
            messageBox.classList.add("success");
            messageBox.style.display = "block";

            setTimeout(() => location.reload(), 1000);
        })
        .catch(err => {
            messageBox.innerText = err.message || "Có lỗi xảy ra";
            messageBox.classList.add("error");
            messageBox.style.display = "block";
        });
}
function deleteUser(id) {
    if (!confirm("Bạn có chắc muốn xóa người dùng này không?")) return;

    fetch("/Admin/DeleteUser", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(id)
    })
        .then(async res => {
            const text = await res.text();
            if (!res.ok) throw new Error(text);
            return text;
        })
        .then(() => {
            alert("Xóa người dùng thành công");
            location.reload();
        })
        .catch(err => {
            alert(err.message || "Không thể xóa người dùng");
        });
}
