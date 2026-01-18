// ===== admin-layout.js =====
document.addEventListener('DOMContentLoaded', function () {
    const sidebar = document.getElementById('adminSidebar');
    const mobileToggle = document.getElementById('mobileToggle');
    const profileBtn = document.getElementById('profileBtn');
    const profileDropdown = document.getElementById('profileDropdown');
    const navItems = document.querySelectorAll('.nav-item');

    if (mobileToggle) {
        mobileToggle.addEventListener('click', function () {
            sidebar.classList.toggle('active');
        });
    }

    if (profileBtn) {
        profileBtn.addEventListener('click', function (e) {
            e.stopPropagation();
            this.classList.toggle('active');
            profileDropdown.classList.toggle('active');
        });
    }

    document.addEventListener('click', function () {
        if (profileDropdown && profileDropdown.classList.contains('active')) {
            profileDropdown.classList.remove('active');
            profileBtn.classList.remove('active');
        }
    });

    const currentPath = window.location.pathname;
    navItems.forEach(item => {
        if (item.getAttribute('href') === currentPath) {
            navItems.forEach(nav => nav.classList.remove('active'));
            item.classList.add('active');
        }
    });
});

// ===== admin-dashboard.js =====
document.addEventListener('DOMContentLoaded', function () {
    function updateTime() {
        const now = new Date();
        const timeString = now.toLocaleString('vi-VN');
        const lastUpdate = document.getElementById('lastUpdate');
        if (lastUpdate) {
            lastUpdate.textContent = timeString;
        }
    }

    updateTime();
    setInterval(updateTime, 60000);

    const statValues = document.querySelectorAll('.stat-value');
    statValues.forEach(stat => {
        const finalValue = stat.textContent;
        const numericValue = parseInt(finalValue.replace(/\D/g, ''));

        if (!isNaN(numericValue)) {
            let current = 0;
            const increment = numericValue / 50;
            const timer = setInterval(() => {
                current += increment;
                if (current >= numericValue) {
                    stat.textContent = finalValue;
                    clearInterval(timer);
                } else {
                    stat.textContent = Math.floor(current);
                }
            }, 20);
        }
    });
});

// ===== admin-users.js =====
let editingUserId = null;

function openAddUserModal() {
    editingUserId = null;
    document.getElementById('modalTitle').textContent = '➕ Thêm Người Dùng Mới';
    document.getElementById('userForm').reset();
    document.getElementById('userModal').classList.add('active');
}

function editUser(userId) {
    editingUserId = userId;
    document.getElementById('modalTitle').textContent = '✏️ Chỉnh Sửa Người Dùng';
    document.getElementById('userModal').classList.add('active');
}

function closeUserModal() {
    document.getElementById('userModal').classList.remove('active');
}

function saveUser() {
    const fullName = document.getElementById('fullName').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    const role = document.getElementById('role').value;

    if (!fullName || !email || !password) {
        alert('Vui lòng điền đầy đủ thông tin!');
        return;
    }

    alert(editingUserId ? 'Cập nhật người dùng thành công!' : 'Thêm người dùng thành công!');
    closeUserModal();
    location.reload();
}

function deleteUser(userId) {
    if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
        alert('Xóa người dùng thành công!');
        location.reload();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const selectAll = document.getElementById('selectAll');
    const rowCheckboxes = document.querySelectorAll('.row-checkbox');
    const searchInput = document.getElementById('searchInput');

    if (selectAll) {
        selectAll.addEventListener('change', function () {
            rowCheckboxes.forEach(cb => cb.checked = this.checked);
        });
    }

    if (searchInput) {
        searchInput.addEventListener('input', function () {
            const searchTerm = this.value.toLowerCase();
            const rows = document.querySelectorAll('#usersTable tbody tr');

            rows.forEach(row => {
                const text = row.textContent.toLowerCase();
                row.style.display = text.includes(searchTerm) ? '' : 'none';
            });
        });
    }
});

// ===== admin-rooms.js =====
let editingRoomId = null;

function openAddRoomModal() {
    editingRoomId = null;
    document.getElementById('modalTitle').textContent = '➕ Thêm Phòng Mới';
    document.getElementById('roomForm').reset();
    document.getElementById('roomModal').classList.add('active');
}

function editRoom(roomId) {
    editingRoomId = roomId;
    document.getElementById('modalTitle').textContent = '✏️ Chỉnh Sửa Phòng';
    document.getElementById('roomModal').classList.add('active');
}

function viewRoom(roomId) {
    alert('Xem chi tiết phòng #' + roomId);
}

function closeRoomModal() {
    document.getElementById('roomModal').classList.remove('active');
}

function saveRoom() {
    const roomName = document.getElementById('roomName').value;
    const branchId = document.getElementById('branchId').value;
    const roomType = document.getElementById('roomType').value;
    const price = document.getElementById('pricePerNight').value;

    if (!roomName || !branchId || !roomType || !price) {
        alert('Vui lòng điền đầy đủ thông tin!');
        return;
    }

    alert(editingRoomId ? 'Cập nhật phòng thành công!' : 'Thêm phòng thành công!');
    closeRoomModal();
    location.reload();
}

function deleteRoom(roomId) {
    if (confirm('Bạn có chắc chắn muốn xóa phòng này?')) {
        alert('Xóa phòng thành công!');
        location.reload();
    }
}

// ===== admin-branches.js =====
let editingBranchId = null;

function openAddBranchModal() {
    editingBranchId = null;
    document.getElementById('modalTitle').textContent = '➕ Thêm Chi Nhánh Mới';
    document.getElementById('branchForm').reset();
    document.getElementById('branchModal').classList.add('active');
}

function editBranch(branchId) {
    editingBranchId = branchId;
    document.getElementById('modalTitle').textContent = '✏️ Chỉnh Sửa Chi Nhánh';
    document.getElementById('branchModal').classList.add('active');
}

function closeBranchModal() {
    document.getElementById('branchModal').classList.remove('active');
}

function saveBranch() {
    const branchName = document.getElementById('branchName').value;
    const description = document.getElementById('branchDescription').value;

    if (!branchName) {
        alert('Vui lòng nhập tên chi nhánh!');
        return;
    }

    alert(editingBranchId ? 'Cập nhật chi nhánh thành công!' : 'Thêm chi nhánh thành công!');
    closeBranchModal();
    location.reload();
}

function deleteBranch(branchId) {
    if (confirm('Bạn có chắc chắn muốn xóa chi nhánh này?\nLưu ý: Tất cả phòng thuộc chi nhánh này cũng sẽ bị xóa!')) {
        alert('Xóa chi nhánh thành công!');
        location.reload();
    }
}