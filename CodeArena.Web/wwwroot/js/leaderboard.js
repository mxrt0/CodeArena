const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/leaderboard")
    .withAutomaticReconnect()
    .build();

let previousEntries = [];

connection.on("LeaderboardUpdated", renderTable);

function getRankDisplay(rank) {
    if (rank === 1) return "🥇";
    if (rank === 2) return "🥈";
    if (rank === 3) return "🥉";
    return rank;
}

function renderTable(entries) {
    const tableBody = document.querySelector("tbody");
    if (!tableBody) return;

    const fragment = document.createDocumentFragment();

    entries.forEach(entry => {
        const prev = previousEntries.find(e => e.userId === entry.userId);
        const hasChanged = !prev || prev.rank !== entry.rank || prev.totalXp !== entry.totalXp;

        const tr = document.createElement("tr");

        if (entry.userId === currentUserId) tr.classList.add("current-user-row", "fw-semibold");
        if (entry.rank === 1) tr.classList.add("top-1");
        else if (entry.rank === 2) tr.classList.add("top-2");
        else if (entry.rank === 3) tr.classList.add("top-3");

        tr.innerHTML = `
            <td>${getRankDisplay(entry.rank)}</td>
            <td>
                <div class="d-flex align-items-center justify-content-center gap-2">
                    <i class="bi bi-person-circle text-muted"></i>
                    <span>${entry.displayName}</span>
                </div>
            </td>
            <td class="fw-bold text-warning">
                <i class="bi bi-lightning-charge me-1"></i>
                ${entry.totalXp}
            </td>
            <td class="d-none d-sm-table-cell">${entry.challengesSolved}</td>
        `;

        fragment.appendChild(tr);
    });

    tableBody.replaceChildren(fragment);

    if (previousEntries.length > 0) {
        tableBody.querySelectorAll("tr").forEach((tr, i) => {
            const entry = entries[i];
            const prev = previousEntries.find(e => e.userId === entry.userId);
            const hasChanged = !prev || prev.rank !== entry.rank || prev.totalXp !== entry.totalXp;

            if (hasChanged) {
                requestAnimationFrame(() => {
                    tr.classList.add("row-updated");
                    tr.addEventListener("animationend", () => {
                        tr.classList.remove("row-updated");
                    }, { once: true });
                });
            }
        });
    }

    previousEntries = entries;
}

connection.start().catch(err => console.error("SignalR error:", err));