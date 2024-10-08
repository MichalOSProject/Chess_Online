﻿using Chess_Online.Server.Models.Pieces;
using System.ComponentModel.DataAnnotations;

namespace Chess_Online.Server.Models.ServiceObjectModels;

public class LobbyInfoModelOutput
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Owner { get; set; }
    public string? PlayerOne { get; set; }
    public string? PlayerTwo { get; set; }
    [Required]
    public bool SwitchedTeam { get; set; }

}
