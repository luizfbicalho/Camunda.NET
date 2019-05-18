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
namespace org.camunda.bpm.engine.variable.impl.value.builder
{

	using PrimitiveValueType = org.camunda.bpm.engine.variable.type.PrimitiveValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;
	using FileValueBuilder = org.camunda.bpm.engine.variable.value.builder.FileValueBuilder;
	using EnsureUtil = org.camunda.commons.utils.EnsureUtil;
	using IoUtil = org.camunda.commons.utils.IoUtil;
	using IoUtilException = org.camunda.commons.utils.IoUtilException;

	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// 
	/// </summary>
	public class FileValueBuilderImpl : FileValueBuilder
	{

	  protected internal FileValueImpl fileValue;

	  public FileValueBuilderImpl(string filename)
	  {
		EnsureUtil.ensureNotNull("filename", filename);
		fileValue = new FileValueImpl(PrimitiveValueType.FILE, filename);
	  }

	  public override FileValue create()
	  {
		return fileValue;
	  }

	  public virtual FileValueBuilder mimeType(string mimeType)
	  {
		fileValue.MimeType = mimeType;
		return this;
	  }

	  public virtual FileValueBuilder file(File file)
	  {
		try
		{
		  return file(IoUtil.fileAsByteArray(file));
		}
		catch (IoUtilException e)
		{
		  throw new System.ArgumentException(e);
		}
	  }

	  public virtual FileValueBuilder file(Stream stream)
	  {
		  try
		  {
			return file(IoUtil.inputStreamAsByteArray(stream));
		  }
		  catch (IoUtilException e)
		  {
			  throw new System.ArgumentException(e);
		  }
	  }

	  public virtual FileValueBuilder file(sbyte[] bytes)
	  {
		fileValue.setValue(bytes);
		return this;
	  }

	  public virtual FileValueBuilder encoding(Charset encoding)
	  {
		fileValue.Encoding = encoding;
		return this;
	  }

	  public virtual FileValueBuilder encoding(string encoding)
	  {
		fileValue.Encoding = encoding;
		return this;
	  }

	  public override FileValueBuilder setTransient(bool isTransient)
	  {
		fileValue.Transient = isTransient;
		return this;
	  }

	}

}