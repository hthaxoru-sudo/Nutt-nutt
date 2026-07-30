// accounts.js - ข้อมูลจำลองและระบบจัดการบัญชี

const MockAccounts = {
    // ฝั่งพนักงาน
    staff: [
        { username: "somchai", password: "123", name: "Somchai", role: "พ่อครัว" },
        { username: "alice", password: "123", name: "Alice", role: "แม่ครัว" }
    ],
    // ฝั่งแอดมิน
    admin: {
        username: "admin",
        password: "123"
    }
};

// ข้อมูลสถานะการเปิดดูออเดอร์ (ป้องกันพนักงานทำงานซ้ำกัน)
let activeStaffViewing = {}; 

// ฟังก์ชันตรวจสอบสิทธิ์พนักงาน
function authenticateStaff(username, password) {
    return MockAccounts.staff.find(s => s.username === username && s.password === password);
}

// ฟังก์ชันตรวจสอบสิทธิ์แอดมิน
function authenticateAdmin(username, password) {
    return MockAccounts.admin.username === username && MockAccounts.admin.password === password;
}
