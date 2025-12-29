from fastapi import FastAPI, HTTPException
import sqlite3
from pydantic import BaseModel
from typing import List, Optional
import threading

app = FastAPI()

# 🔒 GLOBAL LOCK (ไม้ตาย)
db_lock = threading.Lock()

# ---------------- DB ----------------
def get_db_connection():
    conn = sqlite3.connect(
        "DIT109_Final-project.db",
        timeout=10,
        check_same_thread=False
    )
    conn.row_factory = sqlite3.Row
    return conn

# ---------------- Models ----------------
class Player(BaseModel):
    player_id: Optional[int] = None
    name: Optional[str] = None
    health: Optional[int] = 0
    level_id: Optional[int] = 1
    passive_id: Optional[int] = None

class Passive(BaseModel):
    passive_id: Optional[int]
    name: str
    describe: Optional[str] = None

# ---------------- GET ----------------
@app.get("/")
def home():
    return {"message": "DIT109 Database Final project"}

@app.get("/players", response_model=List[Player])
def get_players():
    with db_lock:
        conn = get_db_connection()
        try:
            rows = conn.execute("SELECT * FROM Players").fetchall()
            return [Player(**dict(row)) for row in rows]
        finally:
            conn.close()

@app.get("/player/{player_id}", response_model=Player)
def get_player(player_id: int):
    with db_lock:
        conn = get_db_connection()
        try:
            row = conn.execute(
                "SELECT * FROM Players WHERE player_id = ?",
                (player_id,)
            ).fetchone()

            if not row:
                raise HTTPException(status_code=404, detail="Player not found")

            return Player(**dict(row))
        finally:
            conn.close()

@app.get("/passives", response_model=List[Passive])
def get_passives():
    with db_lock:
        conn = get_db_connection()
        try:
            rows = conn.execute("SELECT * FROM Passives").fetchall()
            return [Passive(**dict(row)) for row in rows]
        finally:
            conn.close()

# ---------------- POST ----------------
@app.post("/player", response_model=Player)
def post_player(player: Player):
    with db_lock:
        conn = get_db_connection()
        try:
            cur = conn.cursor()
            cur.execute(
                """
                INSERT INTO Players (name, health, level_id, passive_id)
                VALUES (?, ?, ?, ?)
                """,
                (player.name, player.health, player.level_id, player.passive_id)
            )
            conn.commit()

            new_id = cur.lastrowid
            row = conn.execute(
                "SELECT * FROM Players WHERE player_id = ?",
                (new_id,)
            ).fetchone()

            return Player(**dict(row))
        finally:
            conn.close()

@app.post("/playersave", response_model=Player)
def post_playersave(player: Player):
    print(player)
    if player.player_id is None:
        raise HTTPException(status_code=400, detail="player_id is required")

    with db_lock:
        conn = get_db_connection()
        try:
            cur = conn.cursor()
            cur.execute(
                """
                UPDATE Players
                SET name = ?, health = ?, level_id = ?, passive_id = ?
                WHERE player_id = ?
                """,
                (
                    player.name,
                    player.health,
                    player.level_id,
                    player.passive_id,
                    player.player_id
                )
            )
            conn.commit()

            if cur.rowcount == 0:
                raise HTTPException(status_code=404, detail="Player not found")

            row = conn.execute(
                "SELECT * FROM Players WHERE player_id = ?",
                (player.player_id,)
            ).fetchone()

            return Player(**dict(row))
        finally:
            conn.close()
