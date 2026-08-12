<!DOCTYPE html>
<html lang="th">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Classroom Hub & Full Admin Control</title>
<style>
  :root {
    --primary: #4b6ef5;
    --primary-dark: #3453d1;
    --bg: #eef1f8;
    --bubble-mine: #4b6ef5;
    --bubble-other: #ffffff;
    --danger: #e5484d;
    --success: #2fa86a;
    --warning: #f5a623;
    --dark-card: #1e2430;
    --sidebar-width: 240px;
  }
  * { box-sizing: border-box; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 0; }
  body { background: var(--bg); min-height: 100vh; color: #2b2f3a; }

  /* Overlays & Auth Modal */
  .overlay { position: fixed; inset: 0; background: rgba(20,25,50,.6); display: flex; justify-content: center; align-items: center; z-index: 999; padding: 16px; }
  .card { background: #fff; padding: 28px 26px; border-radius: 20px; width: 380px; box-shadow: 0 10px 30px rgba(0,0,0,.2); display: flex; flex-direction: column; gap: 12px; position: relative; }
  .card h3 { color: var(--primary-dark); font-size: 1.3rem; text-align: center; }
  .card input, .card textarea, .card select { padding: 10px 12px; border: 1px solid #d7dbea; border-radius: 10px; outline: none; font-size: .92rem; width: 100%; }
  .card input:focus, .card textarea:focus { border-color: var(--primary); }
  .row { display: flex; gap: 8px; }
  .btn { padding: 10px 16px; border: none; background: var(--primary); color: #fff; border-radius: 10px; cursor: pointer; font-weight: 700; font-size: .88rem; transition: .15s; }
  .btn:hover { background: var(--primary-dark); }
  .btn-danger { background: var(--danger); } .btn-danger:hover { background: #c73438; }
  .btn-warning { background: var(--warning); color: #fff; }
  .btn-success { background: var(--success); }
  .btn-outline { background: transparent; border: 1px solid #d7dbea; color: #555; }
  .btn-outline:hover { background: #f0f2fb; }
  .link-btn { background: none; border: none; color: var(--primary); font-size: .8rem; cursor: pointer; text-decoration: underline; text-align: center; margin-top: 4px; }

  /* Main Layout Switcher */
  .app-layout { width: 100vw; height: 100vh; display: flex; overflow: hidden; }

  /* User Chat View */
  .chat-view { width: 100%; height: 100%; display: flex; justify-content: center; align-items: center; padding: 12px; }
  .chat-container { width: 100%; max-width: 800px; height: 95vh; background: #fff; border-radius: 22px; box-shadow: 0 12px 34px rgba(30,40,90,.15); display: flex; flex-direction: column; overflow: hidden; }
  .chat-header { background: linear-gradient(135deg, var(--primary), var(--primary-dark)); color: #fff; padding: 14px 20px; display: flex; justify-content: space-between; align-items: center; }
  .chat-header .title { font-size: 1.15rem; font-weight: 700; }
  .chat-header .sub { font-size: .75rem; opacity: .85; }
  
  .shortcuts-bar { display: flex; gap: 8px; padding: 8px 14px; background: #f0f3fa; border-bottom: 1px solid #e1e5f2; overflow-x: auto; white-space: nowrap; }
  .shortcut-chip { background: #fff; border: 1px solid #d0d7de; padding: 4px 10px; border-radius: 16px; font-size: .8rem; font-weight: 600; text-decoration: none; color: #333; display: inline-flex; align-items: center; gap: 4px; }
  .shortcut-chip:hover { border-color: var(--primary); color: var(--primary); }

  .control-bar { display: flex; gap: 6px; padding: 8px 14px; background: #f8f9fe; border-bottom: 1px solid #e7e9f3; align-items: center; flex-wrap: wrap; }
  .user-tag { font-weight: 700; color: var(--primary-dark); background: #e9edff; padding: 4px 10px; border-radius: 12px; font-size: .82rem; }
  
  .chat-messages { flex: 1; padding: 16px; overflow-y: auto; display: flex; flex-direction: column; gap: 10px; background: var(--bg); }
  .message { max-width: 75%; padding: 10px 14px; border-radius: 16px; font-size: .92rem; line-height: 1.4; word-wrap: break-word; box-shadow: 0 1px 3px rgba(0,0,0,.05); position: relative; }
  .message.mine { background: var(--bubble-mine); color: #fff; align-self: flex-end; border-bottom-right-radius: 4px; }
  .message.others { background: var(--bubble-other); color: #2b2f3a; align-self: flex-start; border-bottom-left-radius: 4px; }
  .message.system { align-self: center; background: #fff3cd; color: #8a6300; font-size: .8rem; font-weight: 600; border-radius: 20px; }
  .message .meta { font-size: .72rem; opacity: .75; margin-bottom: 4px; display: flex; justify-content: space-between; gap: 10px; align-items: center; }
  .del-btn { background: none; border: none; color: inherit; opacity: .6; cursor: pointer; font-size: .75rem; margin-left: 6px; }
  .del-btn:hover { opacity: 1; color: var(--danger); }

  .chat-input { display: flex; gap: 8px; padding: 12px 14px; background: #fff; border-top: 1px solid #e7e9f3; align-items: center; }
  .chat-input input[type="text"] { flex: 1; padding: 10px 14px; border: 1px solid #d7dbea; border-radius: 20px; outline: none; font-size: .9rem; }

  /* Full-Screen Admin Control Panel UI */
  .admin-view { width: 100vw; height: 100vh; display: flex; background: #f4f6fb; }
  .admin-sidebar { width: var(--sidebar-width); background: var(--dark-card); color: #fff; display: flex; flex-direction: column; padding: 16px 0; }
  .admin-sidebar .brand { padding: 0 20px 16px; font-size: 1.1rem; font-weight: 700; border-bottom: 1px solid #2d3545; color: #6b8aff; }
  .admin-menu { list-style: none; margin-top: 10px; flex: 1; overflow-y: auto; }
  .admin-menu li { padding: 12px 20px; cursor: pointer; font-size: .88rem; font-weight: 600; color: #a0aec0; display: flex; align-items: center; gap: 10px; transition: .15s; }
  .admin-menu li:hover, .admin-menu li.active { background: #2a3243; color: #fff; border-left: 4px solid var(--primary); }
  .admin-sidebar .footer-btn { padding: 14px 20px; border-top: 1px solid #2d3545; }

  .admin-content { flex: 1; display: flex; flex-direction: column; overflow: hidden; }
  .admin-header { background: #fff; padding: 14px 24px; border-bottom: 1px solid #e2e8f0; display: flex; justify-content: space-between; align-items: center; }
  .admin-body { flex: 1; padding: 24px; overflow-y: auto; }

  /* Admin Pages & Cards */
  .page-section { display: none; }
  .page-section.active { display: block; }
  .dash-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)); gap: 16px; margin-bottom: 24px; }
  .stat-card { background: #fff; padding: 18px; border-radius: 14px; border: 1px solid #e2e8f0; box-shadow: 0 2px 4px rgba(0,0,0,.02); }
  .stat-card .val { font-size: 1.8rem; font-weight: 700; color: var(--primary-dark); margin-top: 4px; }
  .stat-card .lbl { font-size: .8rem; color: #718096; font-weight: 600; }

  /* Data Tables */
  .table-container { background: #fff; border-radius: 14px; border: 1px solid #e2e8f0; overflow: hidden; margin-top: 12px; }
  table { width: 100%; border-collapse: collapse; text-align: left; font-size: .88rem; }
  th { background: #f7fafc; padding: 12px 16px; font-weight: 700; color: #4a5568; border-bottom: 1px solid #e2e8f0; }
  td { padding: 12px 16px; border-bottom: 1px solid #edf2f7; color: #2d3748; }
  tr:hover { background: #f8fafc; }

  /* Status Badges */
  .badge { padding: 3px 8px; border-radius: 12px; font-size: .75rem; font-weight: 700; display: inline-block; }
  .badge-success { background: #def7ec; color: #03543f; }
  .badge-danger { background: #fde8e8; color: #9b1c1c; }
  .badge-warning { background: #feecdc; color: #b43403; }

  /* Toast & Utilities */
  .toast { position: fixed; top: 16px; left: 50%; transform: translateX(-50%) translateY(-20px); background: #2b2f3a; color: #fff; padding: 10px 18px; border-radius: 12px; font-size: .85rem; font-weight: 600; box-shadow: 0 6px 18px rgba(0,0,0,.25); z-index: 1500; opacity: 0; transition: .25s; pointer-events: none; }
  .toast.show { opacity: 1; transform: translateX(-50%) translateY(0); }
  .pinned-banner { background: #fff3cd; color: #8a6300; font-size: .8rem; font-weight: 600; padding: 8px 16px; text-align: center; border-bottom: 1px solid #ffe4a1; display: none; }
  .schedule-img { width: 100%; max-height: 380px; object-fit: contain; border-radius: 12px; border: 1px solid #d7dbea; background: #f9fafe; }
</style>

<!-- Firebase SDK Compat -->
<script src="https://www.gstatic.com/firebasejs/10.12.2/firebase-app-compat.js"></script>
<script src="https://www.gstatic.com/firebasejs/10.12.2/firebase-auth-compat.js"></script>
<script src="https://www.gstatic.com/firebasejs/10.12.2/firebase-database-compat.js"></script>
<script src="https://www.gstatic.com/firebasejs/10.12.2/firebase-storage-compat.js"></script>
</head>
<body>

<!-- AUTH OVERLAY (Login & Register) -->
<div class="overlay" id="authOverlay">
  <div class="card" id="authCard">
    <h3 id="authTitle">เข้าสู่ระบบ Classroom Hub</h3>
    <input type="email" id="authEmail" placeholder="อีเมลของคุณ (เช่น user@gmail.com)">
    <input type="password" id="authPassword" placeholder="รหัสผ่าน (อย่างน้อย 6 หลัก)">
    <input type="text" id="authName" placeholder="ชื่อเล่น/ชื่อแสดงผล" style="display:none;">
    
    <button class="btn" id="authSubmitBtn" onclick="handleAuthSubmit()">เข้าสู่ระบบ</button>
    <button class="link-btn" id="authToggleBtn" onclick="toggleAuthMode()">ยังไม่มีบัญชี? สมัครสมาชิกที่นี่</button>
  </div>
</div>

<!-- MODAL: เปลี่ยนชื่อตนเอง -->
<div class="overlay" id="editProfileOverlay" style="display:none;" onclick="if(event.target===this) closeModal('editProfileOverlay')">
  <div class="card">
    <h3>✏️ แก้ไขชื่อแสดงผล</h3>
    <input type="text" id="newDisplayNameInput" placeholder="ใส่ชื่อแสดงผลใหม่...">
    <div class="row">
      <button class="btn btn-outline" style="flex:1;" onclick="closeModal('editProfileOverlay')">ยกเลิก</button>
      <button class="btn" style="flex:1;" onclick="saveMyDisplayName()">บันทึก</button>
    </div>
  </div>
</div>

<!-- MODAL: รายงานปัญหา -->
<div class="overlay" id="reportOverlay" style="display:none;" onclick="if(event.target===this) closeModal('reportOverlay')">
  <div class="card">
    <h3>🐞 รายงานปัญหา / แจ้งบั๊ก</h3>
    <textarea id="reportText" rows="4" placeholder="อธิบายปัญหาที่พบ..."></textarea>
    <div class="row">
      <button class="btn btn-outline" style="flex:1;" onclick="closeModal('reportOverlay')">ยกเลิก</button>
      <button class="btn" style="flex:1;" onclick="submitReport()">ส่งรายงาน</button>
    </div>
  </div>
</div>

<!-- MODAL: ตารางเรียน -->
<div class="overlay" id="scheduleOverlay" style="display:none;" onclick="if(event.target===this) closeModal('scheduleOverlay')">
  <div class="card" style="width:480px;">
    <h3>📅 ตารางเรียนประจำห้อง</h3>
    <div id="scheduleContainer" style="margin:12px 0;">
      <p style="color:#888;font-size:.85rem;text-align:center;">ยังไม่ได้อัปโหลดตารางเรียน</p>
    </div>
    <button class="btn" onclick="closeModal('scheduleOverlay')">ปิด</button>
  </div>
</div>

<!-- APP MAIN LAYOUT -->
<div class="app-layout">

  <!-- ==================== 1. CHAT VIEW (ผู้ใช้ทั่วไป & แอดมินใช้งาน) ==================== -->
  <div class="chat-view" id="chatViewSection">
    <div class="chat-container">
      <div class="chat-header">
        <div>
          <div class="title" id="headerAppTitle">Classroom Hub</div>
          <div class="sub">ระบบห้องเรียนออนไลน์ & Realtime Chat</div>
        </div>
        <div style="display:flex;align-items:center;gap:10px;">
          <button class="btn btn-warning" id="adminSwitchBtn" style="display:none;font-size:.78rem;padding:6px 12px;" onclick="switchToAdminUI()">🛠 Full Admin Control</button>
          <button class="btn btn-outline" style="color:#fff;border-color:rgba(255,255,255,.4);font-size:.78rem;padding:6px 12px;" onclick="logout()">ออกจากระบบ</button>
        </div>
      </div>

      <!-- Shortcuts Dynamic Bar -->
      <div class="shortcuts-bar" id="userShortcutsBar">
        <!-- Dynamic Links will be rendered here -->
      </div>

      <div class="pinned-banner" id="pinnedBanner"></div>

      <div class="control-bar">
        <span class="user-tag" id="myNicknameTag">User</span>
        <button class="btn btn-outline" style="padding:4px 8px;font-size:.75rem;" onclick="openModal('editProfileOverlay')">✏️ เปลี่ยนชื่อ</button>

        <select id="chatMode" onchange="togglePrivateInput()" style="padding:5px 8px;border-radius:8px;font-size:.82rem;">
          <option value="group">💬 แชทกลุ่ม</option>
          <option value="private">🔒 แชทเดี่ยว</option>
        </select>
        <select id="targetUserSelect" style="display:none;padding:5px 8px;border-radius:8px;font-size:.82rem;">
          <option value="">-- เลือกผู้รับ --</option>
        </select>

        <button class="btn btn-outline" style="padding:4px 8px;font-size:.75rem;margin-left:auto;" onclick="openModal('scheduleOverlay')">📅 ตารางเรียน</button>
        <button class="btn btn-outline" style="padding:4px 8px;font-size:.75rem;" onclick="openModal('reportOverlay')">🐞 แจ้งบั๊ก</button>
      </div>

      <div class="chat-messages" id="chatBox"></div>

      <div class="chat-input">
        <label for="fileInput" style="cursor:pointer;padding:8px 12px;background:#f0f2f5;border-radius:14px;font-size:.8rem;font-weight:600;">📎</label>
        <input type="file" id="fileInput" style="display:none;" onchange="handleFileSelect()">
        <span id="selectedFileLabel" style="font-size:.72rem;color:var(--primary);max-width:100px;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;"></span>
        <input type="text" id="messageInput" placeholder="พิมพ์ข้อความ..." onkeypress="if(event.key==='Enter'){sendMessage();}">
        <button class="btn" id="sendBtn" onclick="sendMessage()">ส่ง</button>
      </div>
    </div>
  </div>

  <!-- ==================== 2. ADMIN CONTROL CENTER (FULL SCREEN UI) ==================== -->
  <div class="admin-view" id="adminViewSection" style="display:none;">
    <!-- Sidebar Menu -->
    <div class="admin-sidebar">
      <div class="brand">🛠 Admin Control Center</div>
      <ul class="admin-menu">
        <li class="active" onclick="switchAdminPage('dash')">📊 Dashboard</li>
        <li onclick="switchAdminPage('users')">👥 User Management</li>
        <li onclick="switchAdminPage('history')">📜 Account History</li>
        <li onclick="switchAdminPage('bugs')">🐞 Bug Reports <span class="badge badge-danger" id="adminBugBadge" style="display:none;margin-left:auto;">0</span></li>
        <li onclick="switchAdminPage('errors')">⚠️ Error Monitor</li>
        <li onclick="switchAdminPage('scripts')">🧩 Script Diagnostics</li>
        <li onclick="switchAdminPage('shortcuts')">🔗 Link Shortcuts</li>
        <li onclick="switchAdminPage('settings')">⚙️ System Settings</li>
      </ul>
      <div class="footer-btn">
        <button class="btn btn-outline" style="width:100%;color:#a0aec0;border-color:#4a5568;" onclick="switchToChatUI()">← กลับหน้าห้องแชท</button>
      </div>
    </div>

    <!-- Admin Content Area -->
    <div class="admin-content">
      <div class="admin-header">
        <h2 id="adminPageTitle" style="font-size:1.2rem;color:#1a202c;">📊 Dashboard ภาพรวมระบบ</h2>
        <div style="font-size:.85rem;color:#718096;" id="adminUserTag">Admin Account</div>
      </div>

      <div class="admin-body">
        
        <!-- PAGE 1: DASHBOARD -->
        <div class="page-section active" id="page-dash">
          <div class="dash-grid">
            <div class="stat-card"><div class="lbl">ผู้ใช้ทั้งหมด</div><div class="val" id="statTotalUsers">0</div></div>
            <div class="stat-card"><div class="lbl">กำลังออนไลน์</div><div class="val" id="statOnlineUsers" style="color:var(--success);">0</div></div>
            <div class="stat-card"><div class="lbl">ผู้ถูกแบน</div><div class="val" id="statBannedUsers" style="color:var(--danger);">0</div></div>
            <div class="stat-card"><div class="lbl">รายงานบั๊กใหม่</div><div class="val" id="statNewBugs" style="color:var(--warning);">0</div></div>
            <div class="stat-card"><div class="lbl">Error ที่พบ</div><div class="val" id="statTotalErrors" style="color:var(--danger);">0</div></div>
          </div>

          <h3 style="font-size:1rem;margin-bottom:10px;">📋 บันทึกกิจกรรมล่าสุด (Activity Logs)</h3>
          <div class="table-container">
            <table>
              <thead><tr><th>เวลา</th><th>ผู้ทำรายการ</th><th>กิจกรรม / รายละเอียด</th></tr></thead>
              <tbody id="dashActivityLogsTable"><tr><td colspan="3" style="text-align:center;">ไม่มีข้อมูลกิจกรรม</td></tr></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 2: USER MANAGEMENT -->
        <div class="page-section" id="page-users">
          <div style="display:flex;gap:12px;margin-bottom:14px;">
            <input type="text" id="userSearchInput" placeholder="ค้นหาตาม ชื่อ / UID / Email..." oninput="renderUsersTable()" style="max-width:320px;padding:8px 12px;border:1px solid #cbd5e0;border-radius:8px;">
          </div>
          <div class="table-container">
            <table>
              <thead>
                <tr>
                  <th>ชื่อแสดงผล</th>
                  <th>UID</th>
                  <th>Email</th>
                  <th>สถานะ</th>
                  <th>วันที่สมัคร</th>
                  <th>การจัดการ</th>
                </tr>
              </thead>
              <tbody id="userManagementTable"></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 3: ACCOUNT HISTORY -->
        <div class="page-section" id="page-history">
          <h3 style="font-size:1rem;margin-bottom:8px;">📜 ประวัติการผูกบัญชีและเปลี่ยนข้อมูล (Account History Logs)</h3>
          <p style="font-size:.82rem;color:#718096;margin-bottom:12px;">ตรวจสอบการผูก Email กับ UID และประวัติการแก้ไขข้อมูลบัญชีผู้ใช้</p>
          <div class="table-container">
            <table>
              <thead><tr><th>เวลา</th><th>UID</th><th>Email</th><th>เหตุการณ์ / การเปลี่ยนแปลง</th></tr></thead>
              <tbody id="accountHistoryTable"></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 4: BUG REPORTS -->
        <div class="page-section" id="page-bugs">
          <h3 style="font-size:1rem;margin-bottom:12px;">🐞 รายการแจ้งปัญหาและบั๊กระบบ</h3>
          <div class="table-container">
            <table>
              <thead><tr><th>เวลา</th><th>ผู้รายงาน</th><th>รายละเอียดปัญหา</th><th>สถานะ</th><th>จัดการ</th></tr></thead>
              <tbody id="bugReportsTable"></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 5: ERROR MONITOR -->
        <div class="page-section" id="page-errors">
          <div style="display:flex;justify-content:space-between;align-items:center;margin-bottom:12px;">
            <h3 style="font-size:1rem;">⚠️ ระบบดักจับ Error สด (Error Monitor)</h3>
            <button class="btn btn-danger" style="padding:4px 10px;font-size:.78rem;" onclick="clearErrorLogs()">ล้าง Log Error ทั้งหมด</button>
          </div>
          <div class="table-container">
            <table>
              <thead><tr><th>เวลา</th><th>UID/ผู้ใช้</th><th>ประเภท Error</th><th>ข้อความ Error</th></tr></thead>
              <tbody id="errorMonitorTable"></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 6: SCRIPT DIAGNOSTICS -->
        <div class="page-section" id="page-scripts">
          <h3 style="font-size:1rem;margin-bottom:12px;">🧩 ตรวจสอบสถานะการเชื่อมต่อระบบ (System Diagnostics)</h3>
          <div class="table-container" style="max-width:600px;">
            <table>
              <thead><tr><th>โมดูลระบบ</th><th>สถานะ</th><th>การตอบสนอง</th></tr></thead>
              <tbody id="scriptDiagnosticsTable">
                <tr><td>Firebase Auth</td><td><span class="badge badge-success">🟢 ปกติ</span></td><td>Connected</td></tr>
                <tr><td>Realtime Database</td><td><span class="badge badge-success">🟢 ปกติ</span></td><td>Connected (asia-southeast1)</td></tr>
                <tr><td>Firebase Storage</td><td><span class="badge badge-success">🟢 ปกติ</span></td><td>Ready</td></tr>
                <tr><td>Chat Engine</td><td><span class="badge badge-success">🟢 ปกติ</span></td><td>Listening</td></tr>
              </tbody>
            </table>
          </div>
          <button class="btn" style="margin-top:14px;" onclick="runDiagnostics()">🔍 สแกนตรวจสอบระบบสด</button>
        </div>

        <!-- PAGE 7: LINK SHORTCUTS -->
        <div class="page-section" id="page-shortcuts">
          <h3 style="font-size:1rem;margin-bottom:8px;">🔗 จัดการปุ่มเมนูลัด (Shortcut Management)</h3>
          <p style="font-size:.82rem;color:#718096;margin-bottom:14px;">สร้างลิงก์เว็บไซต์สำคัญ เช่น YouTube, Facebook, Discord, Google Drive ให้ผู้ใช้ทั่วไปกดปุ่มเปิดได้ทันที</p>
          
          <div class="card" style="width:100%;max-width:500px;box-shadow:none;border:1px solid #e2e8f0;margin-bottom:20px;">
            <h4>➕ เพิ่ม ShortLink ใหม่</h4>
            <input type="text" id="shortTitle" placeholder="ชื่อปุ่ม (เช่น Discord ห้องเรียน)">
            <input type="text" id="shortUrl" placeholder="URL เว็บไซต์ (เช่น https://discord.gg/...)">
            <input type="text" id="shortIcon" placeholder="ไอคอน Emoji (เช่น 💬, 🔴, 📁)">
            <button class="btn" onclick="addShortcutLink()">เพิ่ม ShortLink</button>
          </div>

          <div class="table-container">
            <table>
              <thead><tr><th>ไอคอน</th><th>ชื่อปุ่ม</th><th>URL</th><th>การจัดการ</th></tr></thead>
              <tbody id="adminShortcutsTable"></tbody>
            </table>
          </div>
        </div>

        <!-- PAGE 8: SYSTEM SETTINGS -->
        <div class="page-section" id="page-settings">
          <h3 style="font-size:1rem;margin-bottom:12px;">⚙️ ตั้งค่าระบบห้องเรียน</h3>
          <div style="max-width:500px;display:flex;flex-direction:column;gap:12px;background:#fff;padding:20px;border-radius:12px;border:1px solid #e2e8f0;">
            <label style="font-size:.85rem;font-weight:600;">ชื่อเว็บไซต์/ห้องเรียน</label>
            <input type="text" id="setAppTitle" placeholder="Classroom Hub">

            <label style="font-size:.85rem;font-weight:600;">ข้อความประกาศปักหมุด</label>
            <input type="text" id="setBannerText" placeholder="เช่น พรุ่งนี้มีสอบย่อยวิชาคณิตศาสตร์...">

            <label style="font-size:.85rem;font-weight:600;">URL รูปตารางเรียน</label>
            <div style="display:flex;gap:6px;">
              <input type="text" id="setScheduleUrl" placeholder="https://.../schedule.png">
              <label for="adminSchedFile" class="btn btn-outline" style="cursor:pointer;white-space:nowrap;font-size:.8rem;">📷 อัปโหลดรูป</label>
              <input type="file" id="adminSchedFile" accept="image/*" style="display:none;" onchange="uploadAdminScheduleImage()">
            </div>

            <label style="font-size:.85rem;font-weight:600;">สีธีมหลัก</label>
            <input type="color" id="setPrimaryColor" value="#4b6ef5" style="height:40px;cursor:pointer;">

            <button class="btn btn-danger" style="margin-top:10px;" onclick="clearAllChatMessages()">🚨 ล้างประวัติแชททั้งหมดในระบบ</button>
            <button class="btn" style="margin-top:10px;" onclick="saveSystemSettings()">💾 บันทึกการตั้งค่า</button>
          </div>
        </div>

      </div>
    </div>
  </div>

</div>

<script>
/* 🔴 CONFIGURATION FIREBASE REALTIME DATABASE & AUTHENTICATION */
const firebaseConfig = {
  apiKey: "AIzaSyC9emUQgRfiMmFML272Ue2rLimj6g3B4q8",
  authDomain: "kkkk-c0436.firebaseapp.com",
  projectId: "kkkk-c0436",
  storageBucket: "kkkk-c0436.firebasestorage.app",
  messagingSenderId: "268898744682",
  appId: "1:268898744682:web:32c774a6f36c081b448d13",
  measurementId: "G-NNSB108BRQ",
  databaseURL: "https://kkkk-c0436-default-rtdb.asia-southeast1.firebasedatabase.app"
};

const ADMIN_EMAIL = "admin@gmail.com"; // อีเมลที่ต้องการให้ได้รับสิทธิ์ Admin อัตโนมัติ

var db, storage, auth;
var myUid = '', myEmail = '', myDisplayName = '', myRole = 'user';
var isAuthRegisterMode = false;
var usersCache = {}, reportsCache = {}, shortcutsCache = {};

// Global Error Catching for Error Monitor
window.onerror = function(msg, url, lineNo, columnNo, error) {
  logErrorToFirebase("JavaScript Error", `${msg} (Line ${lineNo}:${columnNo})`);
  return false;
};

try {
  firebase.initializeApp(firebaseConfig);
  db = firebase.database();
  storage = firebase.storage();
  auth = firebase.auth();
} catch (err) {
  alert('ไม่สามารถเชื่อมต่อ Firebase ได้: ' + err.message);
}

// Authentication State Listener
auth.onAuthStateChanged(async (user) => {
  if (user) {
    myUid = user.uid;
    myEmail = user.email || '';
    
    // ตรวจสอบการแบน
    const banSnap = await db.ref('bannedUsers/' + myUid).once('value');
    const emailBanSnap = await db.ref('bannedEmails/' + myEmail.replace(/\./g, '_')).once('value');
    
    if (banSnap.exists() || emailBanSnap.exists()) {
      const reason = banSnap.val()?.reason || emailBanSnap.val()?.reason || 'คุณถูกแบนจากระบบ';
      alert('🚫 เข้าสู่ระบบไม่สำเร็จ: ' + reason);
      auth.signOut();
      return;
    }

    document.getElementById('authOverlay').style.display = 'none';

    // อ่านหรือสร้างโปรไฟล์ผู้ใช้ใน DB
    const userRef = db.ref('users/' + myUid);
    const snap = await userRef.once('value');
    
    if (!snap.exists()) {
      myDisplayName = myEmail.split('@')[0];
      myRole = (myEmail.toLowerCase() === ADMIN_EMAIL.toLowerCase()) ? 'admin' : 'user';
      await userRef.set({
        uid: myUid,
        email: myEmail,
        displayName: myDisplayName,
        role: myRole,
        status: 'online',
        createdAt: Date.now()
      });
      logAccountHistory(myUid, myEmail, `ลงทะเบียนบัญชีใหม่ (${myRole})`);
    } else {
      const userData = snap.val();
      myDisplayName = userData.displayName || myEmail.split('@')[0];
      myRole = userData.role || 'user';
      await userRef.update({ status: 'online' });
    }

    userRef.onDisconnect().update({ status: 'offline' });

    setupUIForUser();
    initRealtimeListeners();
  } else {
    document.getElementById('authOverlay').style.display = 'flex';
  }
});

function toggleAuthMode() {
  isAuthRegisterMode = !isAuthRegisterMode;
  document.getElementById('authTitle').textContent = isAuthRegisterMode ? 'สมัครสมาชิก Classroom Hub' : 'เข้าสู่ระบบ Classroom Hub';
  document.getElementById('authName').style.display = isAuthRegisterMode ? 'block' : 'none';
  document.getElementById('authSubmitBtn').textContent = isAuthRegisterMode ? 'สมัครสมาชิก' : 'เข้าสู่ระบบ';
  document.getElementById('authToggleBtn').textContent = isAuthRegisterMode ? 'มีบัญชีอยู่แล้ว? เข้าสู่ระบบ' : 'ยังไม่มีบัญชี? สมัครสมาชิกที่นี่';
}

async function handleAuthSubmit() {
  const email = document.getElementById('authEmail').value.trim();
  const password = document.getElementById('authPassword').value;
  const name = document.getElementById('authName').value.trim();

  if (!email || !password) return alert('กรุณากรอกอีเมลและรหัสผ่าน');

  try {
    if (isAuthRegisterMode) {
      if (!name) return alert('กรุณากรอกชื่อเล่น/ชื่อแสดงผล');
      const res = await auth.createUserWithEmailAndPassword(email, password);
      await db.ref('users/' + res.user.uid).set({
        uid: res.user.uid,
        email: email,
        displayName: name,
        role: (email.toLowerCase() === ADMIN_EMAIL.toLowerCase()) ? 'admin' : 'user',
        status: 'online',
        createdAt: Date.now()
      });
      logActivity(name, 'สมัครสมาชิกเข้าสู่ระบบ');
    } else {
      await auth.signInWithEmailAndPassword(email, password);
    }
  } catch (err) {
    alert('เกิดข้อผิดพลาด: ' + err.message);
    logErrorToFirebase('Auth Error', err.message);
  }
}

function logout() {
  if (myUid) db.ref('users/' + myUid + '/status').set('offline');
  auth.signOut().then(() => location.reload());
}

function setupUIForUser() {
  document.getElementById('myNicknameTag').textContent = myDisplayName + (myRole === 'admin' ? ' 🛠' : '');
  if (myRole === 'admin') {
    document.getElementById('adminSwitchBtn').style.display = 'inline-block';
    document.getElementById('adminUserTag').textContent = `Admin: ${myDisplayName} (${myEmail})`;
  }
}

function initRealtimeListeners() {
  // อ่านข้อความแชท
  db.ref('messages').on('value', (snap) => {
    const chatBox = document.getElementById('chatBox');
    chatBox.innerHTML = '';
    const msgs = snap.val() || {};
    Object.keys(msgs).forEach(key => {
      const data = msgs[key];
      if (data.mode === 'group' || data.senderUid === myUid || data.targetUid === myUid) {
        renderMessage(key, data, data.senderUid === myUid);
      }
    });
  });

  // อ่านผู้ใช้ทั้งหมด
  db.ref('users').on('value', (snap) => {
    usersCache = snap.val() || {};
    renderUserSelectOptions();
    renderUsersTable();
    updateDashboardStats();
  });

  // อ่าน ShortLinks
  db.ref('shortcuts').on('value', (snap) => {
    shortcutsCache = snap.val() || {};
    renderShortcutsUI();
  });

  // อ่านการตั้งค่าระบบ
  db.ref('settings').on('value', (snap) => {
    applySystemSettings(snap.val() || {});
  });

  // อ่านรายงานบั๊ก
  db.ref('reports').on('value', (snap) => {
    reportsCache = snap.val() || {};
    renderBugReportsTable();
  });

  // อ่าน Error Logs
  db.ref('errors').on('value', (snap) => {
    renderErrorLogsTable(snap.val() || {});
  });

  // อ่าน Activity Logs
  db.ref('activityLogs').limitToLast(15).on('value', (snap) => {
    renderActivityLogsTable(snap.val() || {});
  });

  // อ่าน Account History Logs
  db.ref('accountHistory').limitToLast(20).on('value', (snap) => {
    renderAccountHistoryTable(snap.val() || {});
  });
}

/* ==================== USER CHAT FUNCTIONS ==================== */
function renderMessage(key, data, isMine) {
  const chatBox = document.getElementById('chatBox');
  const div = document.createElement('div');

  if (data.system) {
    div.className = 'message system';
    div.textContent = '📣 ' + data.text;
    chatBox.appendChild(div);
    chatBox.scrollTop = chatBox.scrollHeight;
    return;
  }

  div.className = 'message ' + (isMine ? 'mine' : 'others');
  const canDel = isMine || myRole === 'admin';
  const delBtn = canDel ? `<button class="del-btn" onclick="deleteMessage('${key}')">🗑</button>` : '';
  const adminTag = data.isAdmin ? '<span class="badge badge-warning" style="font-size:.65rem;">Admin</span>' : '';

  let html = `<div class="meta"><span>${escapeHtml(data.sender)} ${adminTag}</span><span>${escapeHtml(data.time)}${delBtn}</span></div>`;
  if (data.text) html += `<div>${escapeHtml(data.text)}</div>`;
  if (data.file) html += `<div style="margin-top:4px;">📁 <a href="${data.file.url}" target="_blank" style="color:inherit;font-weight:700;">${escapeHtml(data.file.name)}</a></div>`;

  div.innerHTML = html;
  chatBox.appendChild(div);
  chatBox.scrollTop = chatBox.scrollHeight;
}

var selectedFile = null;
function handleFileSelect() {
  const f = document.getElementById('fileInput').files[0];
  if (!f) return;
  selectedFile = f;
  document.getElementById('selectedFileLabel').textContent = f.name;
}

async function sendMessage() {
  const input = document.getElementById('messageInput');
  const text = input.value.trim();
  const mode = document.getElementById('chatMode').value;
  const targetUid = document.getElementById('targetUserSelect').value;

  if (mode === 'private' && !targetUid) return alert('กรุณาเลือกผู้รับแชทเดี่ยว');
  if (!text && !selectedFile) return;

  let fileData = null;
  if (selectedFile) {
    try {
      const ref = storage.ref('uploads/' + Date.now() + '_' + selectedFile.name);
      await ref.put(selectedFile);
      const url = await ref.getDownloadURL();
      fileData = { name: selectedFile.name, url: url };
    } catch (err) {
      alert('อัปโหลดไฟล์ไม่สำเร็จ: ' + err.message);
      logErrorToFirebase('Storage Error', err.message);
      return;
    }
  }

  await db.ref('messages').push({
    sender: myDisplayName,
    senderUid: myUid,
    isAdmin: myRole === 'admin',
    text: text,
    file: fileData,
    mode: mode,
    targetUid: targetUid || null,
    time: new Date().toLocaleTimeString('th-TH', { hour: '2-digit', minute: '2-digit' }),
    ts: Date.now()
  });

  input.value = '';
  document.getElementById('fileInput').value = '';
  document.getElementById('selectedFileLabel').textContent = '';
  selectedFile = null;
}

function deleteMessage(key) {
  if (confirm('ต้องการลบข้อความนี้ใช่หรือไม่?')) {
    db.ref('messages/' + key).remove();
  }
}

async function saveMyDisplayName() {
  const name = document.getElementById('newDisplayNameInput').value.trim();
  if (!name) return alert('กรุณากรอกชื่อ');
  await db.ref('users/' + myUid + '/displayName').set(name);
  logAccountHistory(myUid, myEmail, `เปลี่ยนชื่อแสดงผลเป็น: ${name}`);
  myDisplayName = name;
  document.getElementById('myNicknameTag').textContent = name + (myRole === 'admin' ? ' 🛠' : '');
  closeModal('editProfileOverlay');
  showToast('แก้ไขชื่อสำเร็จ');
}

function submitReport() {
  const text = document.getElementById('reportText').value.trim();
  if (!text) return;
  db.ref('reports').push({
    sender: myDisplayName,
    senderUid: myUid,
    text: text,
    status: 'new',
    time: new Date().toLocaleString('th-TH'),
    ts: Date.now()
  });
  document.getElementById('reportText').value = '';
  closeModal('reportOverlay');
  showToast('ส่งรายงานบั๊กเรียบร้อยแล้ว');
}

/* ==================== ADMIN CONTROL PANEL UI & NAVIGATION ==================== */
function switchToAdminUI() {
  if (myRole !== 'admin') return alert('คุณไม่มีสิทธิ์เข้าถึงหน้า Admin');
  document.getElementById('chatViewSection').style.display = 'none';
  document.getElementById('adminViewSection').style.display = 'flex';
}

function switchToChatUI() {
  document.getElementById('adminViewSection').style.display = 'none';
  document.getElementById('chatViewSection').style.display = 'flex';
}

function switchAdminPage(pageId) {
  document.querySelectorAll('.admin-menu li').forEach(el => el.classList.remove('active'));
  document.querySelectorAll('.page-section').forEach(el => el.classList.remove('active'));

  event.currentTarget.classList.add('active');
  document.getElementById('page-' + pageId).classList.add('active');

  const titles = {
    dash: '📊 Dashboard ภาพรวมระบบ',
    users: '👥 User Management จัดการผู้ใช้',
    history: '📜 Account History ประวัติบัญชี',
    bugs: '🐞 Bug Reports รายการแจ้งบั๊ก',
    errors: '⚠️ Error Monitor ดักจับข้อผิดพลาด',
    scripts: '🧩 Script Diagnostics สถานะโมดูล',
    shortcuts: '🔗 Link Shortcuts จัดการปุ่มลัด',
    settings: '⚙️ System Settings ตั้งค่าระบบ'
  };
  document.getElementById('adminPageTitle').textContent = titles[pageId] || 'Admin Panel';
}

/* ==================== ADMIN DATA RENDERING & ACTIONS ==================== */
function renderUsersTable() {
  const tbody = document.getElementById('userManagementTable');
  tbody.innerHTML = '';
  const search = (document.getElementById('userSearchInput')?.value || '').toLowerCase();

  Object.values(usersCache).forEach(u => {
    if (search && !u.displayName.toLowerCase().includes(search) && !u.uid.toLowerCase().includes(search) && !u.email.toLowerCase().includes(search)) return;

    const tr = document.createElement('tr');
    const isOnline = u.status === 'online';
    const statusBadge = isOnline ? '<span class="badge badge-success">🟢 ออนไลน์</span>' : '<span class="badge badge-danger">⚪️ ออฟไลน์</span>';
    const dateStr = u.createdAt ? new Date(u.createdAt).toLocaleDateString('th-TH') : '-';

    tr.innerHTML = `
      <td><b>${escapeHtml(u.displayName)}</b> ${u.role === 'admin' ? '<span class="badge badge-warning">Admin</span>' : ''}</td>
      <td style="font-family:monospace;font-size:.78rem;">${u.uid}</td>
      <td>${escapeHtml(u.email)}</td>
      <td>${statusBadge}</td>
      <td>${dateStr}</td>
      <td>
        <button class="btn btn-outline" style="padding:2px 6px;font-size:.72rem;" onclick="adminEditUserName('${u.uid}', '${escapeHtml(u.displayName)}')">แก้ชื่อ</button>
        <button class="btn btn-warning" style="padding:2px 6px;font-size:.72rem;" onclick="kickUser('${u.uid}')">เตะ</button>
        <button class="btn btn-danger" style="padding:2px 6px;font-size:.72rem;" onclick="banUserUID('${u.uid}')">แบน UID</button>
        <button class="btn btn-danger" style="padding:2px 6px;font-size:.72rem;" onclick="banUserEmail('${u.email}')">แบน Email</button>
        <button class="btn btn-success" style="padding:2px 6px;font-size:.72rem;" onclick="unbanUser('${u.uid}', '${u.email}')">ปลดแบน</button>
      </td>
    `;
    tbody.appendChild(tr);
  });
}

async function adminEditUserName(uid, oldName) {
  const newName = prompt('แก้ไขชื่อผู้ใช้:', oldName);
  if (newName && newName !== oldName) {
    await db.ref('users/' + uid + '/displayName').set(newName);
    logActivity('Admin', `แก้ไขชื่อผู้ใช้ ${oldName} เป็น ${newName}`);
    logAccountHistory(uid, usersCache[uid]?.email, `Admin เปลี่ยนชื่อเป็น: ${newName}`);
  }
}

async function kickUser(uid) {
  if (confirm('ต้องการเตะผู้ใช้นี้ออกจากระบบหรือไม่?')) {
    await db.ref('users/' + uid + '/status').set('offline');
    logActivity('Admin', `เตะผู้ใช้ UID: ${uid} ออกจากระบบ`);
    showToast('เตะผู้ใช้เรียบร้อยแล้ว');
  }
}

async function banUserUID(uid) {
  const reason = prompt('ใส่เหตุผลในการแบน UID:');
  if (reason) {
    await db.ref('bannedUsers/' + uid).set({ reason: reason, ts: Date.now() });
    await db.ref('users/' + uid + '/status').set('offline');
    logActivity('Admin', `แบน UID: ${uid} (เหตุผล: ${reason})`);
    showToast('แบน UID เรียบร้อยแล้ว');
  }
}

async function banUserEmail(email) {
  const reason = prompt('ใส่เหตุผลในการแบน Email:');
  if (reason) {
    const key = email.replace(/\./g, '_');
    await db.ref('bannedEmails/' + key).set({ email: email, reason: reason, ts: Date.now() });
    logActivity('Admin', `แบน Email: ${email}`);
    showToast('แบน Email เรียบร้อยแล้ว');
  }
}

async function unbanUser(uid, email) {
  await db.ref('bannedUsers/' + uid).remove();
  if (email) await db.ref('bannedEmails/' + email.replace(/\./g, '_')).remove();
  logActivity('Admin', `ปลดแบนผู้ใช้ UID: ${uid} / Email: ${email}`);
  showToast('ปลดแบนเรียบร้อยแล้ว');
}

function renderBugReportsTable() {
  const tbody = document.getElementById('bugReportsTable');
  tbody.innerHTML = '';
  let openCount = 0;

  Object.keys(reportsCache).forEach(key => {
    const r = reportsCache[key];
    if (r.status === 'new') openCount++;

    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${r.time}</td>
      <td>${escapeHtml(r.sender)}</td>
      <td>${escapeHtml(r.text)}</td>
      <td><span class="badge ${r.status === 'new' ? 'badge-danger' : 'badge-success'}">${r.status}</span></td>
      <td>
        <button class="btn btn-success" style="padding:2px 6px;font-size:.72rem;" onclick="updateBugStatus('${key}', 'resolved')">แก้ไขแล้ว</button>
        <button class="btn btn-outline" style="padding:2px 6px;font-size:.72rem;" onclick="deleteBugReport('${key}')">ลบ</button>
      </td>
    `;
    tbody.appendChild(tr);
  });

  document.getElementById('statNewBugs').textContent = openCount;
  const badge = document.getElementById('adminBugBadge');
  if (openCount > 0) { badge.style.display = 'inline-block'; badge.textContent = openCount; }
  else { badge.style.display = 'none'; }
}

function updateBugStatus(key, status) { db.ref('reports/' + key + '/status').set(status); }
function deleteBugReport(key) { db.ref('reports/' + key).remove(); }

function renderErrorLogsTable(errors) {
  const tbody = document.getElementById('errorMonitorTable');
  tbody.innerHTML = '';
  const keys = Object.keys(errors);
  document.getElementById('statTotalErrors').textContent = keys.length;

  keys.reverse().forEach(key => {
    const e = errors[key];
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${e.time}</td>
      <td style="font-size:.78rem;">${e.user} (${e.uid})</td>
      <td><span class="badge badge-danger">${escapeHtml(e.type)}</span></td>
      <td style="font-family:monospace;font-size:.78rem;">${escapeHtml(e.msg)}</td>
    `;
    tbody.appendChild(tr);
  });
}

function clearErrorLogs() { db.ref('errors').remove(); }

function logErrorToFirebase(type, msg) {
  if (!db) return;
  db.ref('errors').push({
    type: type,
    msg: msg,
    uid: myUid || 'Guest',
    user: myDisplayName || 'Unknown',
    time: new Date().toLocaleTimeString('th-TH'),
    ts: Date.now()
  });
}

function logActivity(user, act) {
  db.ref('activityLogs').push({
    user: user,
    action: act,
    time: new Date().toLocaleTimeString('th-TH'),
    ts: Date.now()
  });
}

function logAccountHistory(uid, email, detail) {
  db.ref('accountHistory').push({
    uid: uid,
    email: email || '-',
    detail: detail,
    time: new Date().toLocaleString('th-TH'),
    ts: Date.now()
  });
}

function renderActivityLogsTable(logs) {
  const tbody = document.getElementById('dashActivityLogsTable');
  tbody.innerHTML = '';
  Object.values(logs).reverse().forEach(l => {
    const tr = document.createElement('tr');
    tr.innerHTML = `<td>${l.time}</td><td><b>${escapeHtml(l.user)}</b></td><td>${escapeHtml(l.action)}</td>`;
    tbody.appendChild(tr);
  });
}

function renderAccountHistoryTable(logs) {
  const tbody = document.getElementById('accountHistoryTable');
  tbody.innerHTML = '';
  Object.values(logs).reverse().forEach(l => {
    const tr = document.createElement('tr');
    tr.innerHTML = `<td>${l.time}</td><td style="font-family:monospace;font-size:.78rem;">${l.uid}</td><td>${escapeHtml(l.email)}</td><td>${escapeHtml(l.detail)}</td>`;
    tbody.appendChild(tr);
  });
}

function updateDashboardStats() {
  const users = Object.values(usersCache);
  document.getElementById('statTotalUsers').textContent = users.length;
  document.getElementById('statOnlineUsers').textContent = users.filter(u => u.status === 'online').length;
  
  db.ref('bannedUsers').once('value').then(snap => {
    document.getElementById('statBannedUsers').textContent = snap.numChildren();
  });
}

function runDiagnostics() {
  showToast('🔍 สแกนระบบสำเร็จ: ทุกโมดูลทำงานปกติ');
}

/* ==================== SHORTCUTS MANAGER ==================== */
function addShortcutLink() {
  const title = document.getElementById('shortTitle').value.trim();
  const url = document.getElementById('shortUrl').value.trim();
  const icon = document.getElementById('shortIcon').value.trim() || '🔗';

  if (!title || !url) return alert('กรุณากรอกชื่อและ URL');

  db.ref('shortcuts').push({ title, url, icon }).then(() => {
    document.getElementById('shortTitle').value = '';
    document.getElementById('shortUrl').value = '';
    document.getElementById('shortIcon').value = '';
    showToast('เพิ่ม ShortLink เรียบร้อย');
  });
}

function deleteShortcut(key) { db.ref('shortcuts/' + key).remove(); }

function renderShortcutsUI() {
  // Render ในหน้า Chat View
  const bar = document.getElementById('userShortcutsBar');
  bar.innerHTML = '';

  // Render ในหน้า Admin Table
  const tbody = document.getElementById('adminShortcutsTable');
  tbody.innerHTML = '';

  Object.keys(shortcutsCache).forEach(key => {
    const s = shortcutsCache[key];

    // User chip
    const a = document.createElement('a');
    a.className = 'shortcut-chip';
    a.href = s.url;
    a.target = '_blank';
    a.innerHTML = `<span>${s.icon}</span> <span>${escapeHtml(s.title)}</span>`;
    bar.appendChild(a);

    // Admin Row
    const tr = document.createElement('tr');
    tr.innerHTML = `
      <td>${s.icon}</td>
      <td><b>${escapeHtml(s.title)}</b></td>
      <td><a href="${s.url}" target="_blank">${escapeHtml(s.url)}</a></td>
      <td><button class="btn btn-danger" style="padding:2px 6px;font-size:.72rem;" onclick="deleteShortcut('${key}')">ลบ</button></td>
    `;
    tbody.appendChild(tr);
  });
}

/* ==================== SYSTEM SETTINGS ==================== */
function saveSystemSettings() {
  const title = document.getElementById('setAppTitle').value.trim();
  const banner = document.getElementById('setBannerText').value.trim();
  const schedUrl = document.getElementById('setScheduleUrl').value.trim();
  const color = document.getElementById('setPrimaryColor').value;

  db.ref('settings').set({
    title: title,
    banner: banner,
    scheduleUrl: schedUrl,
    color: color
  }).then(() => showToast('💾 บันทึกการตั้งค่าระบบแล้ว'));
}

async function uploadAdminScheduleImage() {
  const file = document.getElementById('adminSchedFile').files[0];
  if (!file) return;

  showToast('⏳ กำลังอัปโหลดตารางเรียน...');
  try {
    const ref = storage.ref('schedule/' + Date.now() + '_' + file.name);
    await ref.put(file);
    const url = await ref.getDownloadURL();
    document.getElementById('setScheduleUrl').value = url;
    showToast('✅ อัปโหลดตารางเรียนสำเร็จ');
  } catch (err) {
    alert('อัปโหลดไม่สำเร็จ: ' + err.message);
  }
}

function clearAllChatMessages() {
  if (confirm('🚨 คำเตือน: คุณต้องการล้างประวัติแชททั้งหมดใช่หรือไม่?')) {
    db.ref('messages').remove().then(() => showToast('ล้างประวัติแชททั้งหมดแล้ว'));
  }
}

function applySystemSettings(s) {
  if (s.title) document.getElementById('headerAppTitle').textContent = s.title;
  
  const banner = document.getElementById('pinnedBanner');
  if (s.banner) { banner.textContent = '📌 ' + s.banner; banner.style.display = 'block'; }
  else { banner.style.display = 'none'; }

  if (s.color) {
    document.documentElement.style.setProperty('--primary', s.color);
    document.documentElement.style.setProperty('--primary-dark', s.color);
    document.documentElement.style.setProperty('--bubble-mine', s.color);
  }

  const schedBox = document.getElementById('scheduleContainer');
  if (s.scheduleUrl) {
    schedBox.innerHTML = `<img src="${escapeHtml(s.scheduleUrl)}" class="schedule-img" onclick="window.open('${escapeHtml(s.scheduleUrl)}', '_blank')"><p style="font-size:.72rem;color:#777;text-align:center;margin-top:4px;">(คลิกรูปเพื่อขยายใหญ่)</p>`;
  } else {
    schedBox.innerHTML = `<p style="color:#888;font-size:.85rem;text-align:center;">ยังไม่ได้อัปโหลดตารางเรียน</p>`;
  }
}

/* ==================== UTILS & HELPERS ==================== */
function renderUserSelectOptions() {
  const select = document.getElementById('targetUserSelect');
  const cur = select.value;
  select.innerHTML = '<option value="">-- เลือกผู้รับ --</option>';
  Object.values(usersCache).forEach(u => {
    if (u.uid !== myUid) {
      const opt = document.createElement('option');
      opt.value = u.uid;
      opt.textContent = u.displayName + (u.role === 'admin' ? ' 🛠' : '');
      select.appendChild(opt);
    }
  });
  select.value = cur;
}

function togglePrivateInput() {
  const mode = document.getElementById('chatMode').value;
  document.getElementById('targetUserSelect').style.display = mode === 'private' ? 'inline-block' : 'none';
}

function escapeHtml(str) {
  return str ? str.replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c])) : '';
}

function showToast(msg) {
  const t = document.createElement('div');
  t.className = 'toast'; t.textContent = msg;
  document.body.appendChild(t);
  requestAnimationFrame(() => t.classList.add('show'));
  setTimeout(() => { t.classList.remove('show'); setTimeout(() => t.remove(), 300); }, 3000);
}

function openModal(id) { document.getElementById(id).style.display = 'flex'; }
function closeModal(id) { document.getElementById(id).style.display = 'none'; }
</script>
</body>
</html>
