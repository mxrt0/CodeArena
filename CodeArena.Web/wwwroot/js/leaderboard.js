const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/leaderboard")
    .withAutomaticReconnect()
    .build();

connection.on("LeaderboardUpdated", (entries) => {
    renderTable(entries);
});

function renderTable(entries) {
    const tableBody = document.querySelector("tbody");
    tableBody.innerHTML = "";

    entries.forEach(entry => {
        const tr = document.createElement("tr");

        if (entry.userId === currentUserId) {
            tr.classList.add("table-info");
            tr.classList.add("fw-bold");
        }

        tr.innerHTML = `
            <td>${entry.rank}</td>
            <td>${entry.displayName}</td>
            <td>${entry.totalXp}</td>
            <td>${entry.challengesSolved}</td>
        `;
        tableBody.appendChild(tr);
    });
}

connection.start().catch(err => console.error("SignalR error:", err));