export interface Device {
  deviceId: string,
  vendor: string,
  model: string,
  osVersion: string
}

export interface ServiceStatistics {
  sentMessages: number,
  activeDevices: number,
  registeredDevices: number
}

export interface CreateMessageModel {
  deviceId: string | null,
  recipient: string,
  content: string,
  connectionId: string | undefined
}
