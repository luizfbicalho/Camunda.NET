using System;
using System.IO;

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
namespace org.camunda.bpm.engine.impl.cmd
{
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AttachmentEntity = org.camunda.bpm.engine.impl.persistence.entity.AttachmentEntity;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class GetAttachmentContentCmd : Command<Stream>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string attachmentId;

	  public GetAttachmentContentCmd(string attachmentId)
	  {
		this.attachmentId = attachmentId;
	  }

	  public virtual Stream execute(CommandContext commandContext)
	  {
		DbEntityManager dbEntityManger = commandContext.DbEntityManager;
		AttachmentEntity attachment = dbEntityManger.selectById(typeof(AttachmentEntity), attachmentId);

		string contentId = attachment.ContentId;
		if (string.ReferenceEquals(contentId, null))
		{
		  return null;
		}

		ByteArrayEntity byteArray = dbEntityManger.selectById(typeof(ByteArrayEntity), contentId);
		sbyte[] bytes = byteArray.Bytes;

		return new MemoryStream(bytes);
	  }

	}

}