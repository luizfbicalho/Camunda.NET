using System;
using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.engine.task
{

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;


	/// <summary>
	/// Exposes twitter-like feeds for tasks and process instances.
	/// 
	/// <para><strong>Deprecation</strong>
	/// This class has been deprecated as of camunda BPM 7.1. It has been replaced with
	/// the operation log. See <seealso cref="UserOperationLogEntry"/> and <seealso cref="UserOperationLogQuery"/>.</para>
	/// </summary>
	/// <seealso cref= {@link TaskService#getTaskEvents(String)
	/// @author Tom Baeyens </seealso>
	[Obsolete]
	public interface Event
	{

	  /// <summary>
	  /// A user identity link was added with following message parts:
	  /// [0] userId
	  /// [1] identity link type (aka role) 
	  /// </summary>

	  /// <summary>
	  /// A user identity link was added with following message parts:
	  /// [0] userId
	  /// [1] identity link type (aka role) 
	  /// </summary>

	  /// <summary>
	  /// A group identity link was added with following message parts:
	  /// [0] groupId
	  /// [1] identity link type (aka role) 
	  /// </summary>

	  /// <summary>
	  /// A group identity link was added with following message parts:
	  /// [0] groupId
	  /// [1] identity link type (aka role) 
	  /// </summary>

	  /// <summary>
	  /// An user comment was added with the short version of the comment as message. </summary>

	  /// <summary>
	  /// An attachment was added with the attachment name as message. </summary>

	  /// <summary>
	  /// An attachment was deleted with the attachment name as message. </summary>

	  /// <summary>
	  /// Indicates the type of of action and also indicates the meaning of the parts as exposed in <seealso cref="getMessageParts()"/> </summary>
	  string Action {get;}

	  /// <summary>
	  /// The meaning of the message parts is defined by the action as you can find in <seealso cref="getAction()"/> </summary>
	  IList<string> MessageParts {get;}

	  /// <summary>
	  /// The message that can be used in case this action only has a single message part. </summary>
	  string Message {get;}

	  /// <summary>
	  /// reference to the user that made the comment </summary>
	  string UserId {get;}

	  /// <summary>
	  /// time and date when the user made the comment </summary>
	  DateTime Time {get;}

	  /// <summary>
	  /// reference to the task on which this comment was made </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// reference to the process instance on which this comment was made </summary>
	  string ProcessInstanceId {get;}

	}

	public static class Event_Fields
	{
	  public const string ACTION_ADD_USER_LINK = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_USER_LINK;
	  public const string ACTION_DELETE_USER_LINK = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_USER_LINK;
	  public const string ACTION_ADD_GROUP_LINK = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_GROUP_LINK;
	  public const string ACTION_DELETE_GROUP_LINK = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_GROUP_LINK;
	  public const string ACTION_ADD_COMMENT = "AddComment";
	  public const string ACTION_ADD_ATTACHMENT = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_ATTACHMENT;
	  public const string ACTION_DELETE_ATTACHMENT = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_ATTACHMENT;
	}

}