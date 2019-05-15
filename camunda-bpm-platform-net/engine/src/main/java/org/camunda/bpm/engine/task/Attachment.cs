﻿using System;

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



	/// <summary>
	/// Any type of content that is be associated with
	/// a task or with a process instance.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface Attachment
	{

	  /// <summary>
	  /// unique id for this attachment </summary>
	  string Id {get;}

	  /// <summary>
	  /// free user defined short (max 255 chars) name for this attachment </summary>
	  string Name {get;set;}


	  /// <summary>
	  /// long (max 255 chars) explanation what this attachment is about in context of the task and/or process instance it's linked to. </summary>
	  string Description {get;set;}


	  /// <summary>
	  /// indication of the type of content that this attachment refers to. Can be mime type or any other indication. </summary>
	  string Type {get;}

	  /// <summary>
	  /// reference to the task to which this attachment is associated. </summary>
	  string TaskId {get;}

	  /// <summary>
	  /// reference to the process instance to which this attachment is associated. </summary>
	  string ProcessInstanceId {get;}

	  /// <summary>
	  /// the remote URL in case this is remote content.  If the attachment content was 
	  /// <seealso cref="TaskService#createAttachment(String, String, String, String, String, java.io.InputStream) uploaded with an input stream"/>, 
	  /// then this method returns null and the content can be fetched with <seealso cref="TaskService#getAttachmentContent(String)"/>. 
	  /// </summary>
	  string Url {get;}

	  /// <summary>
	  /// The time when the attachment was created. </summary>
	  DateTime CreateTime {get;}

	  /// <summary>
	  /// reference to the root process instance id of the process instance on which this attachment was made </summary>
	  string RootProcessInstanceId {get;}

	  /// <summary>
	  /// The time the historic attachment will be removed. </summary>
	  DateTime RemovalTime {get;}

	}

}