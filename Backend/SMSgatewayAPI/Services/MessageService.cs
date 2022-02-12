using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Entities;
using DAL.Enums;
using IdGenerator;
using Microsoft.EntityFrameworkCore;
using SMSgatewayAPI.Models;

namespace SMSgatewayAPI.Services;

public interface IMessageService
{
    Task<MessageModel> GetMessage(ulong messageId);

    /// <summary>
    /// Gets list of all messages sent by a device with certain ID
    /// </summary>
    /// <param name="deviceId">ID of the device</param>
    /// <returns>List of all messages</returns>
    Task<List<MessageModel>> GetMessages(string deviceId);

    /// <summary>
    /// Gets list of all sent messages
    /// </summary>
    /// <returns>List of all messages</returns>
    Task<List<MessageModel>> GetAllMessages();

    /// <summary>
    /// Creates a message on the database
    /// </summary>
    /// <param name="model">Message to create</param>
    /// <returns>Created message</returns>
    Task<MessageModel> CreateMessage(CreateMessageModel model);

    /// <summary>
    /// Updates the message on the database with specified message state
    /// </summary>
    /// <param name="message">Message to update</param>
    /// <param name="state">New state of the message</param>
    Task UpdateMessage(MessageModel message, MessageState state);
}

public class MessageService : IMessageService
{
    private readonly DatabaseContext _context;
    private readonly SnowflakeGenerator _snowflakeGenerator;

    public MessageService(DatabaseContext context, SnowflakeGenerator snowflakeGenerator)
    {
        _context = context;
        _snowflakeGenerator = snowflakeGenerator;
    }

    public async Task<MessageModel> GetMessage(ulong messageId) => await _context.Messages.FindAsync(messageId);

    public async Task<List<MessageModel>> GetMessages(string deviceId)
    {
        // Return list of messages sent by device with certain ID
        return await _context.Messages
            .Where(model => model.DeviceId == deviceId)
            .OrderByDescending(model => model.SentAt)
            .ToListAsync();
    }

    public async Task<List<MessageModel>> GetAllMessages() => await _context.Messages.ToListAsync();

    public async Task<MessageModel> CreateMessage(CreateMessageModel model)
    {
        // Creates a new instance of the message
        var message = new MessageModel
        {
            MessageId = _snowflakeGenerator.NextLong(),
            Content = model.Content,
            Recipient = model.Recipient,
            DeviceId = model.DeviceId,
            State = MessageState.Waiting,
            SentAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            ConnectionId = model.ConnectionId
        };

        // Saves the messages in the database
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        return message;
    }

    public async Task UpdateMessage(MessageModel message, MessageState state)
    {
        // Changes state of the message and processed at date
        message.State = state;
        message.Updated = true;
        message.ProceededAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Updates the message in the database
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }
}