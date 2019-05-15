using System;

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
	using Picture = org.camunda.bpm.engine.identity.Picture;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using IdentityInfoEntity = org.camunda.bpm.engine.impl.persistence.entity.IdentityInfoEntity;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	/// <summary>
	/// @author Daniel Meyer
	/// @author Tom Baeyens
	/// </summary>
	[Serializable]
	public class SetUserPictureCmd : Command<Void>
	{

	  private const long serialVersionUID = 1L;
	  protected internal string userId;
	  protected internal Picture picture;


	  public SetUserPictureCmd(string userId, Picture picture)
	  {
		this.userId = userId;
		this.picture = picture;
	  }

	  public virtual Void execute(CommandContext commandContext)
	  {
		ensureNotNull("userId", userId);

		IdentityInfoEntity pictureInfo = commandContext.IdentityInfoManager.findUserInfoByUserIdAndKey(userId, "picture");

		if (pictureInfo != null)
		{
		  string byteArrayId = pictureInfo.Value;
		  if (!string.ReferenceEquals(byteArrayId, null))
		  {
			commandContext.ByteArrayManager.deleteByteArrayById(byteArrayId);
		  }

		}
		else
		{
		  pictureInfo = new IdentityInfoEntity();
		  pictureInfo.UserId = userId;
		  pictureInfo.Key = "picture";
		  commandContext.DbEntityManager.insert(pictureInfo);
		}

		ByteArrayEntity byteArrayEntity = new ByteArrayEntity(picture.MimeType, picture.Bytes, ResourceTypes.REPOSITORY);

		commandContext.ByteArrayManager.insertByteArray(byteArrayEntity);

		pictureInfo.Value = byteArrayEntity.Id;

		return null;
	  }

	}

}