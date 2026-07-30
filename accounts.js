// accounts.js - รองรับพนักงานและแอดมินหลายคน

const MockAccounts = {
    // รายชื่อพนักงานครัวทั้งหมด
    staff: [
        { id: "somchai", password: "123", name: "สมชาย (พ่อครัว)", role: "พ่อครัว" },
        { id: "alice", password: "456", name: "อลิซ (แม่ครัว)", role: "แม่ครัว" },
        { id: "pracha", password: "789", name: "ประชา (ผู้ช่วยกุ๊ก)", role: "ผู้ช่วย" }
    ],
    // รายชื่อแอดมินทั้งหมด
    admin: [
        { id: "admin_boss", password: "admin123", name: "คุณบอส (เจ้าของร้าน)", role: "Admin" },
        { id: "admin_manager", password: "manager123", name: "คุณจัดการ (ผู้จัดการ)", role: "Admin" }
    ]
};

let activeStaffViewing = {}; 

// ฟังก์ชันตรวจสอบสิทธิ์พนักงาน (เช็คจากชื่อที่เลือก และ Password)
function authenticateStaff(selectedId, password) {
    return MockAccounts.staff.find(s => s.id === selectedId && s.password === password);
}

// ฟังก์ชันตรวจสอบสิทธิ์แอดมิน (เช็คจากชื่อที่เลือก และ Password)
function authenticateAdmin(selectedId, password) {
    return MockAccounts.admin.find(a => a.id === selectedId && a.password === password);
}
